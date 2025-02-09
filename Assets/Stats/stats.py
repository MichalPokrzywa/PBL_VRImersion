import json
import os
import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
import seaborn as sns
from tqdm import tqdm

# Ustawienia wykresów
sns.set(style="whitegrid")
plt.rcParams["figure.figsize"] = (10, 6)

def load_data(file_path):
    """Wczytuje dane z pliku JSON."""
    with open(file_path, 'r') as f:
        return json.load(f)

def aggregate_data(data):
    """Agreguje dane dotyczące prędkości, rotacji i pauz z jednej sesji."""
    aggregated_data = {
        "Session Duration (s)": data.get("timeDuration"),
        "Completed": data.get("completed"),
        "Max Velocity (x, y, z)": data.get("maxVelocity"),
        "Avg Velocity (x, y, z)": data.get("avgVelocity"),
        "Total Head Rotation (degrees)": data.get("totalHeadRotation"),
        "Total Body Rotation (degrees)": data.get("totalBodyRotation"),
        "Max Head Rotation Speed (degrees/s)": data.get("maxHeadRotationSpeed"),
        "Max Body Rotation Speed (degrees/s)": data.get("maxBodyRotationSpeed"),
        "Avg Head Rotation Speed (degrees/s)": data.get("avgHeadRotationSpeed"),
        "Avg Body Rotation Speed (degrees/s)": data.get("avgBodyRotationSpeed"),
        "Number of Pauses": len(data.get("pauseData", [])),
        "Total Pause Duration (s)": sum(p.get("duration", 0) for p in data.get("pauseData", [])),
        "Number of Teleports": len(data.get("teleportResumeTime", [])),
        "Dangerous Place Resumes": len(data.get("dangerousPlaceResumeTime", []))
    }
    return aggregated_data

def process_stats(base_dir):
    """
    Przeszukuje folder base_dir, który zawiera podfoldery dla poszczególnych osób.
    W każdym folderze przeszukuje pliki JSON zaczynające się od "Level_".
    Zwraca strukturę:
      {osoba: {poziom: [lista zagregowanych danych sesji], ...}, ...}
    """
    stats_data = {}
    for person in os.listdir(base_dir):
        person_path = os.path.join(base_dir, person)
        if os.path.isdir(person_path):
            stats_data[person] = {}  # folder danej osoby
            for file_name in os.listdir(person_path):
                if file_name.endswith('.json') and file_name.startswith("Level_"):
                    # Przykładowa nazwa: "Level_E_Seed123_06.02.2025_19_38_21.json"
                    # Zakładamy, że drugi element po "_" to nazwa poziomu.
                    parts = file_name.split('_')
                    if len(parts) >= 2:
                        level_name = parts[1]
                        file_path = os.path.join(person_path, file_name)
                        try:
                            data = load_data(file_path)
                            aggregated = aggregate_data(data)
                        except Exception as e:
                            print(f"Błąd przy przetwarzaniu pliku {file_path}: {e}")
                            continue
                        if level_name not in stats_data[person]:
                            stats_data[person][level_name] = []
                        stats_data[person][level_name].append(aggregated)
    return stats_data

def aggregated_data_to_df(stats_data):
    """
    Przekształca dane ze struktury:
      {osoba: {poziom: [sesje, ...], ...}, ...}
    do DataFrame, w którym każda sesja ma kolumny: Person, Level, oraz zagregowane statystyki.
    """
    rows = []
    for person, levels in stats_data.items():
        for level, sessions in levels.items():
            for session in sessions:
                row = {"Person": person, "Level": level}
                row.update(session)
                rows.append(row)
    df = pd.DataFrame(rows)
    return df

def add_group_columns(df):
    """
    Na podstawie nazwy folderu (kolumna 'Person') dodaje kolumny:
      - 'Sickness': "chory" jeśli '_ch' znajduje się w nazwie, w przeciwnym razie "zdrowy"
      - 'Experience': "nie doświadczony" jeśli '_nd' znajduje się w nazwie, w przeciwnym razie "doświadczony"
    """
    def get_sickness(person):
        return "chory" if "_ch" in person.lower() else "zdrowy"
    
    def get_experience(person):
        return "nie doświadczony" if "_nd" in person.lower() else "doświadczony"
    
    df["Sickness"] = df["Person"].apply(get_sickness)
    df["Experience"] = df["Person"].apply(get_experience)
    
    # Dodatkowo tworzymy kolumnę binarną (1 = chory, 0 = zdrowy) przydatną do analizy korelacji
    df["Sickness_bin"] = df["Sickness"].apply(lambda x: 1 if x=="chory" else 0)
    return df

def display_basic_stats(df):
    """Wyświetla podstawowe statystyki DataFrame oraz liczbę sesji dla grup."""
    print("=== Podstawowe statystyki DataFrame ===")
    print(df.describe(include='all'))
    print("\nLiczba sesji dla każdej osoby i poziomu:")
    print(df.groupby(["Person", "Level"]).size())
    print("\nLiczba sesji wg. stanu VR choroby:")
    print(df.groupby("Sickness").size())
    print("\nLiczba sesji wg. doświadczenia:")
    print(df.groupby("Experience").size())

def plot_comparisons_interactive(df):
    """
    Interaktywna karuzela wykresów – klawisze ← i → przełączają kolejne wykresy.
    Dodano porównania według VR choroby, doświadczenia oraz ukończenia poziomów.
    """

    # Dodajemy kolumnę z informacją o ukończeniu poziomu
    # (przyjmujemy, że kolumna "Completed" zawiera wartość True/False)
    df["LevelStatus"] = df["Completed"].apply(lambda x: "ukończony" if x else "nieukończony")

    # Definiujemy kilka funkcji, z których każda rysuje inny wykres.
    # Każda funkcja zaczyna się od wyczyszczenia aktualnej figury (plt.clf()).
    
    def plot_session_by_sickness():
        plt.clf()
        sns.barplot(data=df, x="Sickness", y="Session Duration (s)", hue="Level", errorbar="sd")
        plt.title("Czas sesji wg. VR choroby i poziomu")
        plt.legend(loc='best')

    def plot_session_by_experience():
        plt.clf()
        sns.barplot(data=df, x="Experience", y="Session Duration (s)", hue="Level", errorbar="sd")
        plt.title("Czas sesji wg. doświadczenia w VR i poziomu")
        plt.legend(loc='best')

    def plot_completed_status():
        plt.clf()
        sns.barplot(data=df, x="LevelStatus", y="Session Duration (s)", hue="Level", errorbar="sd")
        plt.title("Czas sesji wg. ukończenia poziomu")
        plt.legend(loc='best')

    def plot_correlation():
        plt.clf()  # czyścimy główną figurę
        # Ustawiamy subplots w głównej figurze (1 wiersz, 2 kolumny)
        ax1 = plt.subplot(1, 2, 1)
        ax2 = plt.subplot(1, 2, 2)
        
        numeric_cols = [
            "Session Duration (s)",
            "Total Head Rotation (degrees)",
            "Total Body Rotation (degrees)",
            "Avg Head Rotation Speed (degrees/s)",
            "Avg Body Rotation Speed (degrees/s)",
            "Sickness_bin"
        ]
        df_numeric = df[numeric_cols].dropna()
        healthy_df = df_numeric[df_numeric["Sickness_bin"] == 0]
        sick_df    = df_numeric[df_numeric["Sickness_bin"] == 1]
        
        sns.heatmap(healthy_df.corr(), annot=True, cmap="coolwarm", ax=ax1)
        ax1.set_title("Korelacja - zdrowi")
        sns.heatmap(sick_df.corr(), annot=True, cmap="coolwarm", ax=ax2)
        ax2.set_title("Korelacja - chorzy")
        
        plt.suptitle("Macierze korelacji")
        plt.draw()


    def plot_scatter():
        plt.clf()
        sns.scatterplot(data=df, x="Session Duration (s)", y="Total Head Rotation (degrees)",
                        hue="Sickness", style="Level", s=100)
        plt.title("Rotacja głowy vs czas sesji")
        plt.legend(loc='best')

    def plot_pause_comparison():
        plt.clf()
        # Tworzymy 3 subploty w głównej figurze (1 wiersz, 3 kolumny)
        ax1 = plt.subplot(1, 3, 1)
        ax2 = plt.subplot(1, 3, 2)
        ax3 = plt.subplot(1, 3, 3)
        
        sns.boxplot(data=df, x="Sickness", y="Dangerous Place Resumes", ax=ax1)
        ax1.set_title("Dangerous Place Resumes")
        sns.boxplot(data=df, x="Sickness", y="Number of Pauses", ax=ax2)
        ax2.set_title("Number of Pauses")
        sns.boxplot(data=df, x="Sickness", y="Total Pause Duration (s)", ax=ax3)
        ax3.set_title("Total Pause Duration (s)")
        
        plt.suptitle("Parametry pauz: zdrowi vs. chorzy")
        plt.tight_layout(rect=[0, 0, 1, 0.95])
        plt.draw()
    
    def plot_pause_comparison_experience():
        plt.clf()
        # Tworzymy 3 subploty w głównej figurze (1 wiersz, 3 kolumny)
        ax1 = plt.subplot(1, 3, 1)
        ax2 = plt.subplot(1, 3, 2)
        ax3 = plt.subplot(1, 3, 3)
        
        sns.boxplot(data=df, x="Experience", y="Dangerous Place Resumes", ax=ax1)
        ax1.set_title("Dangerous Place Resumes")
        
        sns.boxplot(data=df, x="Experience", y="Number of Pauses", ax=ax2)
        ax2.set_title("Number of Pauses")
        
        sns.boxplot(data=df, x="Experience", y="Total Pause Duration (s)", ax=ax3)
        ax3.set_title("Total Pause Duration (s)")
        
        plt.suptitle("Parametry pauz: doświadczony vs. nie doświadczony")
        plt.tight_layout(rect=[0, 0, 1, 0.95])
        plt.draw()

    # Umieszczamy funkcje w liście – będą przełączane klawiszami strzałek
    plot_funcs = [plot_session_by_sickness,
                  plot_session_by_experience,
                  plot_completed_status,
                  plot_correlation,
                  plot_scatter,
                  plot_pause_comparison,
                  plot_pause_comparison_experience]

    # Używamy listy jednoelementowej do przechowania aktualnego indeksu (aby można było modyfikować zmienną z wnętrza funkcji)
    current_plot = [0]

    fig = plt.figure()
    plt.subplots_adjust(top=0.88)  # zostaw miejsce na tytuł główny jeśli potrzebne

    def on_key(event):
        if event.key == 'right':
            current_plot[0] = (current_plot[0] + 1) % len(plot_funcs)
            plt.clf()
            plot_funcs[current_plot[0]]()
            plt.draw()
        elif event.key == 'left':
            current_plot[0] = (current_plot[0] - 1) % len(plot_funcs)
            plt.clf()
            plot_funcs[current_plot[0]]()
            plt.draw()

    fig.canvas.mpl_connect('key_press_event', on_key)

    # Rysujemy pierwszy wykres
    plot_funcs[current_plot[0]]()
    plt.suptitle("Interaktywna karuzela wykresów (←/→ aby przełączać)")
    plt.show()

def display_stats(stats_data):
    """Wyświetla zebrane dane w przejrzystej formie."""
    for person, levels in stats_data.items():
        print(f"=== Statystyki dla osoby: {person} ===")
        for level, sessions in levels.items():
            print(f"  Poziom: {level}")
            for i, session in enumerate(sessions, start=1):
                print(f"    Sesja {i}:")
                for key, value in session.items():
                    print(f"      {key}: {value}")
                print()  # nowa linia między sesjami
        print("-" * 40)

def load_samples(base_dir):
    """
    Przeszukuje folder base_dir (który zawiera podfoldery dla osób) i dla każdego pliku JSON
    zaczynającego się od "Level_" wyciąga dane z klucza 'samples'.
    Zwraca DataFrame z kolumnami:
      Person, Level, File, time,
      headRotationAngle, bodyRotationAngle, headRotationSpeed, bodyRotationSpeed,
      velocity_x, velocity_y, velocity_z, velocity_magnitude.
    Dodaje pasek postępu przy przetwarzaniu plików.
    """
    sample_rows = []
    all_files = []

    # Zbieramy wszystkie pliki spełniające kryteria
    for person in os.listdir(base_dir):
        person_path = os.path.join(base_dir, person)
        if os.path.isdir(person_path):
            for file_name in os.listdir(person_path):
                if file_name.endswith('.json') and file_name.startswith("Level_"):
                    all_files.append((person, file_name, os.path.join(person_path, file_name)))
                    
    # Przetwarzamy pliki z użyciem paska postępu
    for person, file_name, file_path in tqdm(all_files, desc="Processing JSON files", unit="file"):
        try:
            data = load_data(file_path)
        except Exception as e:
            print(f"Error loading {file_path}: {e}")
            continue
        
        samples = data.get("samples", [])
        for sample in samples:
            t = sample.get("time")
            if t is None:
                continue
            # Dane o rotacji
            rotation = sample.get("rotation", {})
            headRotAngle = rotation.get("headRotationAngle")
            bodyRotAngle = rotation.get("bodyRotationAngle")
            headRotSpeed = rotation.get("headRotationSpeed")
            bodyRotSpeed = rotation.get("bodyRotationSpeed")
            
            # Dane o prędkości – z pola "movement" -> "velocity"
            movement = sample.get("movement", {})
            velocity = movement.get("velocity", {})
            vx = velocity.get("x")
            vy = velocity.get("y")
            vz = velocity.get("z")
            if vx is not None and vy is not None and vz is not None:
                vmag = (vx**2 + vy**2 + vz**2) ** 0.5
            else:
                vmag = None
            
            # Wyciągamy Level z nazwy pliku (np. z "Level_E_Seed123_06.02.2025_19_38_21.json" wyciągamy "E")
            parts = file_name.split('_')
            level = parts[1] if len(parts) >= 2 else "Unknown"
            
            sample_rows.append({
                "Person": person,
                "Level": level,
                "File": file_name,
                "time": t,
                "headRotationAngle": headRotAngle,
                "bodyRotationAngle": bodyRotAngle,
                "headRotationSpeed": headRotSpeed,
                "bodyRotationSpeed": bodyRotSpeed,
                "velocity_x": vx,
                "velocity_y": vy,
                "velocity_z": vz,
                "velocity_magnitude": vmag
            })
            
    df_samples = pd.DataFrame(sample_rows)
    return df_samples


from tqdm import tqdm

def plot_combined_interactive(df_samples):
    """
    Tworzy interaktywne wykresy z możliwością przewijania plików strzałkami.
    Dla każdego pliku wyświetla trzy wykresy obok siebie:
    - prędkość vs czas,
    - rotacja głowy vs czas,
    - rotacja ciała vs czas.
    """
    # Przygotowanie danych
    df = df_samples.copy()
    df["time_sec"] = df["time"].astype(int)
    
    # Lista unikalnych plików (sesji)
    files = df["File"].unique()
    if len(files) == 0:
        print("Brak danych do wyświetlenia.")
        return
    
    # Przechowujemy dane zgrupowane dla każdego pliku
    file_data = {}
    for file in files:
        df_file = df[df["File"] == file]
        grouped = df_file.groupby("time_sec").agg({
            "headRotationAngle": "mean",
            "bodyRotationAngle": "mean",
            "velocity_magnitude": "mean"
        }).reset_index()
        file_data[file] = grouped
    
    # Inicjalizacja interfejsu
    current_file_idx = [0]  # przechowuje aktualny indeks (użyte jako lista, by można modyfikować)
    fig = plt.figure(figsize=(18, 5))
    plt.subplots_adjust(top=0.85, wspace=0.3)
    
    def update_plot(idx):
        """Rysuje wykresy dla pliku o podanym indeksie."""
        plt.clf()  # wyczyść całą figurę
        file = files[idx]
        data = file_data[file]
        
        # Pobierz nazwę folderu z kolumny "Person"
        folder_name = df[df["File"] == file]["Person"].iloc[0]
        
        # Wykres prędkości
        ax1 = plt.subplot(1, 3, 1)
        ax1.plot(data["time_sec"], data["velocity_magnitude"], 'g-', label="Prędkość")
        ax1.set_title(f"Prędkość - {file}")
        ax1.set_xlabel("Czas (s)")
        ax1.set_ylabel("Moduł prędkości")
        ax1.legend()
        
        # Wykres rotacji głowy
        ax2 = plt.subplot(1, 3, 2)
        ax2.plot(data["time_sec"], data["headRotationAngle"], 'b-', label="Głowa")
        ax2.set_title(f"Rotacja głowy - {file}")
        ax2.set_xlabel("Czas (s)")
        ax2.set_ylabel("Kąt (°)")
        ax2.legend()
        
        # Wykres rotacji ciała
        ax3 = plt.subplot(1, 3, 3)
        ax3.plot(data["time_sec"], data["bodyRotationAngle"], 'r-', label="Ciało")
        ax3.set_title(f"Rotacja ciała - {file}")
        ax3.set_xlabel("Czas (s)")
        ax3.set_ylabel("Kąt (°)")
        ax3.legend()

        # Dodaj napis z nazwą folderu w lewym górnym rogu
        plt.text(0.01, 0.98, f"Folder: {folder_name}", 
                 transform=fig.transFigure, fontsize=12, verticalalignment='top', bbox=dict(facecolor='white', alpha=0.8))
        
        plt.suptitle(f"Plik {idx+1}/{len(files)}: {file} (←/→ aby przewijać)", fontsize=14)
        plt.draw()
    
    def on_key(event):
        """Obsługa klawiszy strzałek."""
        if event.key == 'right':
            current_file_idx[0] = (current_file_idx[0] + 1) % len(files)
            update_plot(current_file_idx[0])
        elif event.key == 'left':
            current_file_idx[0] = (current_file_idx[0] - 1) % len(files)
            update_plot(current_file_idx[0])
    
    fig.canvas.mpl_connect('key_press_event', on_key)
    update_plot(current_file_idx[0])  # początkowe wyświetlenie
    plt.show()

    
if __name__ == "__main__":
    # Ustal ścieżkę do folderu Stats (zakładamy, że skrypt jest uruchamiany z folderu, w którym są podfoldery osób)
    base_dir = os.path.dirname(__file__)
    
    # Przetwórz dane ze wszystkich podfolderów
    stats_data = process_stats(base_dir)

    display_stats(stats_data)
    
    # Konwertuj dane do DataFrame
    df = aggregated_data_to_df(stats_data)
    
    # Dodaj kolumny grupujące: Sickness oraz Experience
    df = add_group_columns(df)
    
    # Wyświetl podstawowe statystyki
    display_basic_stats(df)
    
    # Utwórz wykresy porównawcze
    plot_comparisons_interactive(df)

    df_samples = load_samples(base_dir)
    
    if df_samples.empty:
        print("Brak danych sampli do analizy.")
    else:
        plot_combined_interactive(df_samples)  # zmieniona nazwa funkcji

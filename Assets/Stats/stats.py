import json
import os
import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns

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

def plot_comparisons(df):
    """Tworzy wykresy porównujące statystyki wg. VR choroby i doświadczenia."""
    
    # 1. Zestawienie czasu przejścia poziomów: osoby chory vs. zdrowy
    plt.figure()
    sns.barplot(data=df, x="Sickness", y="Session Duration (s)", hue="Level", ci="sd")
    plt.title("Średni czas trwania sesji wg. stanu VR (chory/zdrowy) oraz poziomu")
    plt.tight_layout()
    plt.show()
    
    # 2. Porównanie czasów: osoby doświadczone vs. niedoświadczone
    plt.figure()
    sns.barplot(data=df, x="Experience", y="Session Duration (s)", hue="Level", ci="sd")
    plt.title("Średni czas trwania sesji wg. doświadczenia w VR oraz poziomu")
    plt.tight_layout()
    plt.show()
    
    # Wybieramy kolumny numeryczne, w tym kolumnę binarną Sickness (0 = zdrowy, 1 = chory)
    numeric_cols = [
        "Session Duration (s)",
        "Total Head Rotation (degrees)",
        "Total Body Rotation (degrees)",
        "Avg Head Rotation Speed (degrees/s)",
        "Avg Body Rotation Speed (degrees/s)",
        "Sickness_bin"
    ]

    # Usuwamy ewentualne NaN-y
    df_numeric = df[numeric_cols].dropna()

    # Filtrujemy dane na osoby zdrowe i chore
    healthy_df = df_numeric[df_numeric["Sickness_bin"] == 0]
    sick_df    = df_numeric[df_numeric["Sickness_bin"] == 1]

    # Liczymy macierz korelacji dla każdej grupy
    healthy_corr = healthy_df.corr()
    sick_corr    = sick_df.corr()

    # Tworzymy dwie podmacierze w jednej figurze
    fig, (ax1, ax2) = plt.subplots(nrows=2, figsize=(10, 12))

    sns.heatmap(healthy_corr, annot=True, cmap="coolwarm", ax=ax1)
    ax1.set_title("Macierz korelacji dla osób zdrowych")

    sns.heatmap(sick_corr, annot=True, cmap="coolwarm", ax=ax2)
    ax2.set_title("Macierz korelacji dla osób chorych")

    plt.tight_layout()
    plt.show()


    # Opcjonalnie: scatter plot pokazujący zależność czasu sesji od rotacji głowy,
    # z rozróżnieniem stanu choroby
    plt.figure()
    sns.scatterplot(data=df, x="Session Duration (s)", y="Total Head Rotation (degrees)",
                    hue="Sickness", style="Level", s=100)
    plt.title("Total Head Rotation vs. Session Duration - podział wg. VR choroby")
    plt.tight_layout()
    plt.show()
      # Wykres 1: Średni czas trwania sesji według osób i poziomów
    plt.figure()
    sns.barplot(data=df, x="Person", y="Session Duration (s)", hue="Level", ci=None)
    plt.title("Średni czas trwania sesji według osób i poziomów")
    plt.tight_layout()
    plt.show()

    # Wykres 2: Scatter plot - Całkowita rotacja głowy vs. czas sesji, kolor: osoba, kształt: poziom
    plt.figure()
    sns.scatterplot(data=df, x="Session Duration (s)", y="Total Head Rotation (degrees)",
                    hue="Person", style="Level", s=100)
    plt.title("Total Head Rotation vs Session Duration")
    plt.tight_layout()
    plt.show()

    # Wykres 3: Boxplot - Średnia prędkość rotacji głowy dla poszczególnych osób i poziomów
    plt.figure()
    sns.boxplot(data=df, x="Person", y="Avg Head Rotation Speed (degrees/s)", hue="Level")
    plt.title("Average Head Rotation Speed by Person and Level")
    plt.tight_layout()
    plt.show()

     # 4. Porównanie Dangerous Place Resumes, Number of Pauses oraz Total Pause Duration między zdrowymi a chorymi
    # Możesz wybrać osobne wykresy lub umieścić je w jednym oknie z wieloma subplotami.
    
    # Opcja A: osobne wykresy
    plt.figure()
    sns.boxplot(data=df, x="Sickness", y="Dangerous Place Resumes")
    plt.title("Porównanie Dangerous Place Resumes: zdrowi vs. chorzy")
    plt.tight_layout()
    plt.show()

    plt.figure()
    sns.boxplot(data=df, x="Sickness", y="Number of Pauses")
    plt.title("Porównanie Number of Pauses: zdrowi vs. chorzy")
    plt.tight_layout()
    plt.show()

    plt.figure()
    sns.boxplot(data=df, x="Sickness", y="Total Pause Duration (s)")
    plt.title("Porównanie Total Pause Duration (s): zdrowi vs. chorzy")
    plt.tight_layout()
    plt.show()
    
    # Opcja B: wykresy w jednym oknie (subplots)
    # Jeśli wolisz mieć wszystkie trzy porównania w jednym oknie, odkomentuj poniższy blok:
    """
    fig, axes = plt.subplots(1, 3, figsize=(18, 6))
    sns.boxplot(data=df, x="Sickness", y="Dangerous Place Resumes", ax=axes[0])
    axes[0].set_title("Dangerous Place Resumes")
    
    sns.boxplot(data=df, x="Sickness", y="Number of Pauses", ax=axes[1])
    axes[1].set_title("Number of Pauses")
    
    sns.boxplot(data=df, x="Sickness", y="Total Pause Duration (s)", ax=axes[2])
    axes[2].set_title("Total Pause Duration (s)")
    
    fig.suptitle("Porównanie parametrów pauz: zdrowi vs. chorzy", fontsize=16)
    plt.tight_layout(rect=[0, 0, 1, 0.95])
    plt.show()
    """

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
    plot_comparisons(df)

using System;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation
{
    /// <summary>
    /// The <see cref="TeleportationProvider"/> is responsible for moving the XR Origin
    /// to the desired location on the user's request.
    /// </summary>
    public class CustomTeleportationProvider : TeleportationProvider
    {
        public static Action OnTeleported;

        protected override void Update()
        {
            if (!validRequest)
                return;

            base.Update();

            OnTeleported?.Invoke();
        }
    }
}

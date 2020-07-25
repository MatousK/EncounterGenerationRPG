using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Combat;
using Assets.Scripts.Cutscenes;
using Assets.Scripts.DungeonGenerator;
using Assets.Scripts.Effects;
using Assets.Scripts.Extension;
using Assets.Scripts.GameFlow;
using Assets.Scripts.Input;
using Assets.Scripts.Movement.Pathfinding;
using UnityEngine;

namespace Assets.Scripts.Environment
{
    /// <summary>
    /// Information about the orientation of these doors.
    /// </summary>
    public enum DoorOrientation
    {
        /// <summary>
        /// The doors are oriented vertically.
        /// </summary>
        Vertical,
        /// <summary>
        /// The doors are oriented horizontally.
        /// </summary>
        Horizontal
    }
    /// <summary>
    /// Represents doors on the map that can be opened and closed. Will update pathfinding information when entered and will play the cutscene on doors open.
    /// Requires that the object has the <see cref="InteractableObject"/> to detect that the character interacted with the doors.
    /// </summary>
    [ExecuteAfter(typeof(PathfindingMapController))]
    public class Doors : MonoBehaviour
    {
        /// <summary>
        /// If true, these doors should close automatically in combat.
        /// </summary>
        public bool CloseInCombat = true;
        /// <summary>
        /// Whether the doors connect rooms on a Y or X axis.
        /// </summary>
        public DoorOrientation Orientation;
        /// <summary>
        /// List of rooms these doors are connecting.
        /// </summary>
        public List<int> ConnectingRooms = new List<int>();
        /// <summary>
        /// All game objects which should be enabled iff these doors are opened.
        /// </summary>
        public List<GameObject> OpenDoorsObjects = new List<GameObject>();
        /// <summary>
        /// All game objects which should be enabled iff these doors are closed.
        /// </summary>
        public List<GameObject> ClosedDoorsObjects = new List<GameObject>();
        /// <summary>
        /// One of these audio clips will play when the door are opened.
        /// </summary>
        public List<AudioClip> OpenDoorsAudioClips = new List<AudioClip>();
        /// <summary>
        /// The grid on which these doors are spawned.
        /// </summary>
        public Grid MapGrid;
        /// <summary>
        /// Responsible for pathfinding data about the current map.
        /// The doors notify them when they are opened or closed to mark the door spaces as either passable or unpassable.
        /// </summary>
        private PathfindingMapController pathfindingMapController;
        /// <summary>
        /// Provides information about the rooms in the game. 
        /// When the doors are opened, sets the <see cref="RoomInfo.isExplored"/> on the appropriate room.
        /// </summary>
        private RoomsLayout roomsLayout;
        /// <summary>
        /// The class that can play the enter room cutscene when the doors are opened.
        /// </summary>
        private CutsceneManager cutsceneManager;
        /// <summary>
        /// Class which knows about all the combatants. Used to determine whether there is a combat active (doors are not interactable during combat.)
        /// </summary>
        private CombatantsManager combatantstManager;
        /// <summary>
        /// If true, the player has already at least once opened these doors. Used to open these doors again after autoclosing for combat.
        /// </summary>
        private bool didPlayerOpenDoors;
        /// <summary>
        /// The hero who opened this door if any.
        /// </summary>
        private Hero doorOpener;
        /// <summary>
        /// If true, the doors are currently opened. Do not use directly, use <see cref="IsOpened"/>.
        /// </summary>
        private bool isOpened;
        /// <summary>
        /// Notifies the rest of the game about the game being reloaded or a game over.
        /// Used to reset the doors on reload.
        /// </summary>
        private GameStateManager gameStateManager;
        /// <summary>
        /// If true, the doors are currently opened.
        /// </summary>
        public bool IsOpened
        {
            get => isOpened;
            set
            {
                if (IsOpened == value) return;
                isOpened = value;
                OnDoorOpenedChanged();
            }
        }

        /// <summary>
        /// Start is called before the first frame update. Finds dependencies in the scene and subscribes to events.
        /// </summary>
        void Start()
        {
            gameStateManager = FindObjectOfType<GameStateManager>();
            combatantstManager = FindObjectOfType<CombatantsManager>();
            cutsceneManager = FindObjectOfType<CutsceneManager>();
            roomsLayout = FindObjectOfType<RoomsLayout>();
            pathfindingMapController = FindObjectOfType<PathfindingMapController>();
            MapGrid = MapGrid != null ? MapGrid : FindObjectOfType<Grid>();
            GetComponent<InteractableObject>().OnInteractionTriggered += OnDoorsInteractedWith;
            gameStateManager.GameReloaded += GameStateManager_GameReloaded;
            OnDoorOpenedChanged();
        }
        /// <summary>
        /// Called every frame. Updates whether the doors are currently opened or closed.
        /// </summary>
        private void Update()
        {
            UpdateOpened();
        }
        /// <summary>
        /// Updates whether the doors are currently opened or closed.
        /// Most importantly it closes the doors in combat.
        /// </summary>
        private void UpdateOpened()
        {
            IsOpened = didPlayerOpenDoors && (!CloseInCombat || !combatantstManager.IsCombatActive);
        }
        /// <summary>
        /// Called when a character walks to the doors and interacts with them.
        /// Opens the doors, sets the room as explored and starts the enter room cutscene if necessary,
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">The character who opened the doors.</param>
        private void OnDoorsInteractedWith(object sender, Hero e)
        {
            if (didPlayerOpenDoors)
            {
                // Never open doors more than once.
                return;
            }
            // Play relevant audio
            var audioClipToPlay = OpenDoorsAudioClips.GetRandomElementOrDefault();
            var audioSource = GetComponent<AudioSource>();
            if (audioSource != null && audioClipToPlay != null)
            {
                audioSource.PlayOneShot(audioClipToPlay);
            }
            // Open the doors, set the room as explored.
            doorOpener = e;
            didPlayerOpenDoors = true;
            UpdateOpened();
            foreach (var roomIndex in ConnectingRooms)
            {
                if (!roomsLayout.Rooms[roomIndex].IsExplored)
                {
                    roomsLayout.Rooms[roomIndex].SetRoomExplored(true, this);
                    var openDoorsCutscene = cutsceneManager.InstantiateCutscene<EnterRoomCutscene>();
                    openDoorsCutscene.DoorOpener = doorOpener;
                    openDoorsCutscene.EnteredRoom = roomsLayout.Rooms[roomIndex];
                    openDoorsCutscene.OpenedDoors = this;
                    cutsceneManager.PlayCutscene(openDoorsCutscene);
                }
            }
        }
        /// <summary>
        /// Called to update the pathfinding map about whether the door squares are currently passable or not.
        /// Must be called whenever the doors open or close. It is also called to when the map is initialized to store the initial state of the doors.
        /// Because of a bit of hell in order of initialization, the <see cref="PathfindingMapController"/> is initialized after the doors, so it must call this method manually.
        /// That is why this method is public.
        /// </summary>
        /// <param name="map">The pathfinding map that should be updated.</param>
        public void UpdatePathfindingMap(PathfindingMap map)
        {
            if (map == null)
            {
                return;
            }
            // Doors are 2x2, so the center is right in the middle. So 0.5 in any two directions is also occupied by these doors.
            SetMapSquareIsPassable(map, -0.5f, -0.5f);
            SetMapSquareIsPassable(map, -0.5f, 0.5f);
            SetMapSquareIsPassable(map, 0.5f, -0.5f);
            SetMapSquareIsPassable(map, 0.5f, 0.5f);
        }
        /// <summary>
        /// On the given pathfinding map, set that the specified door square is passable or not based on <see cref="IsOpened"/>.
        /// </summary>
        /// <param name="map">The map to update</param>
        /// <param name="xOffset">How much offcenter on X axis is the square that should be made passable or impassable.</param>
        /// <param name="yOffset">How much offcenter on Y axis is the square that should be made passable or impassable.</param>
        void SetMapSquareIsPassable(PathfindingMap map, float xOffset, float yOffset)
        {
            var coordinates = MapGrid.WorldToCell(new Vector3(transform.position.x + xOffset, transform.position.y + yOffset, transform.position.z));
            map.SetSquareIsPassable(coordinates.x, coordinates.y, IsOpened);
        }
        /// <summary>
        /// Called when the doors open or close.
        /// Updates the visual representation of the doors, the pathfinding map and the shimmer effect on the doors.
        /// </summary>
        void OnDoorOpenedChanged()
        {
            foreach (var openedDoor in OpenDoorsObjects)
            {
                openedDoor.SetActive(IsOpened);
            }
            foreach (var closedDoor in ClosedDoorsObjects)
            {
                closedDoor.SetActive(!IsOpened);
            }
            if (pathfindingMapController != null)
            {
                UpdatePathfindingMap(pathfindingMapController.Map);
            }

            var shimmerEffect = GetComponentInChildren<InteractableObjectShimmer>();
            if (shimmerEffect != null)
            {
                shimmerEffect.ObjectAlreadyUsed = IsOpened;
            }
        }
        /// <summary>
        /// When the game is reloaded, close all doors.
        /// </summary>
        /// <param name="sender">Sender of this event.</param>
        /// <param name="e">Arguments of the event.</param>
        private void GameStateManager_GameReloaded(object sender, System.EventArgs e)
        {
            didPlayerOpenDoors = false;
            IsOpened = false;
        }
    }
}
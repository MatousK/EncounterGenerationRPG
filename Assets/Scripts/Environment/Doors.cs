﻿using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Combat;
using Assets.Scripts.Cutscenes;
using Assets.Scripts.DungeonGenerator;
using Assets.Scripts.Extension;
using Assets.Scripts.GameFlow;
using Assets.Scripts.Input;
using Assets.Scripts.Movement.Pathfinding;
using UnityEngine;

namespace Assets.Scripts.Environment
{
    public enum DoorOrientation
    {
        Vertical,
        Horizontal
    }
    /// <summary>
    /// Represents doors on the map that can be opened and closed. Will update pathfinding information when entered and will play the cutscene on doors open.
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

        public List<AudioClip> OpenDoorsAudioClips = new List<AudioClip>();

        public Grid MapGrid;
        private PathfindingMapController pathfindingMapController;
        private RoomsLayout roomsLayout;
        private CutsceneManager cutsceneManager;
        private CombatantsManager combatantstManager;
        // If true, the player has already at least once opened these doors. Used to open these doores again after autoclosing for combat.
        private bool didPlayerOpenDoors;
        // The hero who opened this door if any.
        private Hero doorOpener;
        private bool isOpened;
        private GameStateManager gameStateManager;
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

        // Start is called before the first frame update
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

        private void Update()
        {
            UpdateOpened();
        }

        private void UpdateOpened()
        {
            IsOpened = didPlayerOpenDoors && (!CloseInCombat || !combatantstManager.IsCombatActive);
        }

        private void OnDoorsInteractedWith(object sender, Hero e)
        {
            if (didPlayerOpenDoors)
            {
                // Never open doors more than once.
                return;
            }

            var audioClipToPlay = OpenDoorsAudioClips.GetRandomElementOrDefault();
            var audioSource = GetComponent<AudioSource>();
            if (audioSource != null && audioClipToPlay != null)
            {
                audioSource.PlayOneShot(audioClipToPlay);
            }
            doorOpener = e;
            didPlayerOpenDoors = true;
            UpdateOpened();
            foreach (var roomIndex in ConnectingRooms)
            {
                if (!roomsLayout.Rooms[roomIndex].IsExplored)
                {
                    roomsLayout.Rooms[roomIndex].SetRoomExplored(this);
                    var openDoorsCutscene = cutsceneManager.InstantiateCutscene<EnterRoomCutscene>();
                    openDoorsCutscene.DoorOpener = doorOpener;
                    openDoorsCutscene.EnteredRoom = roomsLayout.Rooms[roomIndex];
                    openDoorsCutscene.OpenedDoors = this;
                    cutsceneManager.PlayCutscene(openDoorsCutscene);
                }
            }
        }

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

        void SetMapSquareIsPassable(PathfindingMap map, float xOffset, float yOffset)
        {
            var coordinates = MapGrid.WorldToCell(new Vector3(transform.position.x + xOffset, transform.position.y + yOffset, transform.position.z));
            map.SetSquareIsPassable(coordinates.x, coordinates.y, IsOpened);
        }

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
        }

        private void GameStateManager_GameReloaded(object sender, System.EventArgs e)
        {
            didPlayerOpenDoors = false;
            IsOpened = false;
        }
    }
}
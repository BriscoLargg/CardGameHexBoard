﻿using Game.Ui;
using HexCardGame;
using HexCardGame.SharedData;
using Tools.Input.Mouse;
using UnityEngine;

namespace Tools.UI.Card
{
    [RequireComponent(typeof(Collider2D)), RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(IMouseInput))]
    public class UiCardHandComponent : MonoBehaviour, IUiCard
    {
        //--------------------------------------------------------------------------------------------------------------

        #region Components

        SpriteRenderer[] IUiCardComponents.Renderers => MyRenderers;
        SpriteRenderer IUiCardComponents.MyRenderer => MyRenderer;
        Collider2D IUiCardComponents.Collider => MyCollider;
        Rigidbody2D IUiCardComponents.Rigidbody => MyRigidbody;
        IMouseInput IUiCardComponents.Input => MyInput;
        UiCardHandSelector IUiCardComponents.HandSelector => HandSelector;

        #endregion

        #region Transform

        public UiMotionBaseCard Movement { get; private set; }
        public UiMotionBaseCard Rotation { get; private set; }
        public UiMotionBaseCard Scale { get; private set; }

        #endregion

        #region Properties

        public string Name => gameObject.name;
        public UiCardParameters CardConfigsParameters => cardConfigsParameters;
        [SerializeField] private SpriteRenderer artwork;
        [SerializeField] public UiCardParameters cardConfigsParameters;
        private UiCardHandFsm Fsm { get; set; }
        private Transform MyTransform { get; set; }
        private Collider2D MyCollider { get; set; }
        private SpriteRenderer[] MyRenderers { get; set; }
        private SpriteRenderer MyRenderer { get; set; }
        private Rigidbody2D MyRigidbody { get; set; }
        private IMouseInput MyInput { get; set; }
        private UiCardHandSelector HandSelector { get; set; }
        public MonoBehaviour MonoBehaviour => this;
        public Camera MainCamera => Camera.main;
        public bool IsDragging => Fsm.IsCurrent<UiCardDrag>();
        public bool IsHovering => Fsm.IsCurrent<UiCardHover>();
        public bool IsDisabled => Fsm.IsCurrent<UiCardDisable>();
        public bool IsUser => id == SeatType.Bottom;
        private SeatType id;
        private UiGameDataAccess dataAccess;

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region Transform

        public void RotateTo(Vector3 rotation, float speed)
        {
            Rotation.Execute(rotation, speed);
        }

        public void MoveTo(Vector3 position, float speed, float delay)
        {
            Movement.Execute(position, speed, delay);
        }

        public void MoveToWithZ(Vector3 position, float speed, float delay)
        {
            Movement.Execute(position, speed, delay, true);
        }

        public void ScaleTo(Vector3 scale, float speed, float delay)
        {
            Scale.Execute(scale, speed, delay);
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region Operations

        public void Hover()
        {
            Fsm.Hover();
        }

        public void Disable()
        {
            Fsm.Disable();
        }

        public void Enable()
        {
            Fsm.Enable();
        }

        public void Select()
        {
            // to avoid the player selecting enemy's cards
            if (!dataAccess.IsMyTurn(id))
                return;

            HandSelector.SelectCard(this);
            Fsm.Select();
        }

        public void Unselect()
        {
            Fsm.Unselect();
        }

        public void Draw()
        {
            Fsm.Draw();
        }

        public void Discard()
        {
            Fsm.Discard();
        }

        public void SetAndUpdateView(ICardData data, SeatType seatType, UiGameDataAccess access)
        {
            artwork.sprite = data.Artwork;
            id = seatType;
            dataAccess = access;
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region Unity Callbacks

        public void Initialize()
        {
            //components
            MyTransform = transform;
            MyCollider = GetComponent<Collider2D>();
            MyRigidbody = GetComponent<Rigidbody2D>();
            MyInput = GetComponent<IMouseInput>();
            HandSelector = transform.GetComponentInParent<UiCardHandSelector>();
            MyRenderers = GetComponentsInChildren<SpriteRenderer>();
            MyRenderer = GetComponent<SpriteRenderer>();

            //transform
            Scale = new UiMotionScaleCard(this);
            Movement = new UiMotionMovementCard(this);
            Rotation = new UiMotionRotationCard(this);

            (MyCollider as BoxCollider2D).size = cardConfigsParameters.CardSize.Value;
            //fsm
            Fsm = new UiCardHandFsm(MainCamera, CardConfigsParameters, this);
        }


        private void Update()
        {
            Fsm?.Update();
            Movement?.Update();
            Rotation?.Update();
            Scale?.Update();
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
    }
}
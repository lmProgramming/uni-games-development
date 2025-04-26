// ------------------------------------------ 
// BasicFPCC.cs
// a basic first person character controller
// with jump, crouch, run, slide
// 2020-10-04 Alucard Jay Kay 
// ------------------------------------------ 

// source : 
// https://discussions.unity.com/t/855344
// Brackeys FPS controller base : 
// https://www.youtube.com/watch?v=_QajrabyTJc
// smooth mouse look : 
// https://discussions.unity.com/t/710168/2
// ground check : (added isGrounded)
// https://gist.github.com/jawinn/f466b237c0cdc5f92d96
// run, crouch, slide : (added check for headroom before un-crouching)
// https://answers.unity.com/questions/374157/character-controller-slide-action-script.html
// interact with rigidbodies : 
// https://docs.unity3d.com/2018.4/Documentation/ScriptReference/CharacterController.OnControllerColliderHit.html

// ** SETUP **
// Assign the BasicFPCC object to its own Layer
// Assign the Layer Mask to ignore the BasicFPCC object Layer
// CharacterController (component) : Center => X 0, Y 1, Z 0
// Main Camera (as child) : Transform : Position => X 0, Y 1.7, Z 0
// (optional GFX) Capsule primitive without collider (as child) : Transform : Position => X 0, Y 1, Z 0
// alternatively : 
// at the end of this script is a Menu Item function to create and autoconfigure a BasicFPCC object
// GameObject -> 3D Object -> BasicFPCC

using System;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace BasicFPCC
{
    [RequireComponent(typeof(CharacterController))]
    public class BasicFPCC : MonoBehaviour
    {
        [Header("Layer Mask")]
        [Tooltip("Layer Mask for sphere/raycasts. Assign the Player object to a Layer, then Ignore that layer here.")]
        public LayerMask castingMask; // Layer mask for casts. You'll want to ignore the player.

        [Header("Main Camera")] [Tooltip("Drag the FPC Camera here")]
        public Transform cameraTx; // Main Camera, as child of BasicFPCC object

        [Header("Optional Player Graphic")] [Tooltip("optional capsule to visualize player in scene view")]
        public Transform playerGfx; // optional capsule graphic object

        [Header("Inputs")] [Tooltip("Disable if sending inputs from an external script")]
        public bool useLocalInputs = true;

        [Space(5)] public string axisLookHorizontal = "Mouse X"; // Mouse to Look

        public string axisLookVertical = "Mouse Y"; // 
        public string axisMoveHorizontal = "Horizontal"; // WASD to Move
        public string axisMoveVertical = "Vertical"; // 
        public KeyCode keyRun = KeyCode.LeftShift; // Left Shift to Run
        public KeyCode keyCrouch = KeyCode.LeftControl; // Left Control to Crouch
        public KeyCode keyJump = KeyCode.Space; // Space to Jump
        public KeyCode keySlide = KeyCode.F; // F to Slide (only when running)
        public KeyCode keyToggleCursor = KeyCode.BackQuote; // ` to toggle lock cursor (aka [~] console key)


        // Input Variables that can be assigned externally
        // the cursor can also be manually locked or freed by calling the public void SetLockCursor( bool doLock )
        [HideInInspector] public float inputLookX; //
        [HideInInspector] public float inputLookY; //
        [HideInInspector] public float inputMoveX; // range -1f to +1f
        [HideInInspector] public float inputMoveY; // range -1f to +1f
        [HideInInspector] public bool inputKeyRun; // is key Held
        [HideInInspector] public bool inputKeyCrouch; // is key Held
        [HideInInspector] public bool inputKeyDownJump; // is key Pressed
        [HideInInspector] public bool inputKeyDownSlide; // is key Pressed
        [HideInInspector] public bool inputKeyDownCursor; // is key Pressed

        [Header("Look Settings")] public float mouseSensitivityX = 2f; // speed factor of look X

        public float mouseSensitivityY = 2f; // speed factor of look Y

        [Tooltip("larger values for less filtering, more responsiveness")]
        public float
            mouseSnappiness = 20f; // default was 10f; larger values of this cause less filtering, more responsiveness

        public bool invertLookY; // toggle invert look Y
        public float clampLookY = 90f; // maximum look up/down angle

        [Header("Move Settings")] public float crouchSpeed = 3f; // crouching movement speed

        public float walkSpeed = 7f; // regular movement speed
        public float runSpeed = 12f; // run movement speed
        public float slideSpeed = 14f; // slide movement speed
        public float slideDuration = 2.2f; // duration of slide
        public float gravity = -9.81f; // gravity / fall rate
        public float jumpHeight = 2.5f; // jump height

        [Header("Grounded Settings")]
        [Tooltip(
            "The starting position of the isGrounded spherecast. Set to the sphereCastRadius plus the CC Skin Width. Enable showGizmos to visualize.")]
        // this should be just above the base of the cc, in the amount of the skin width (in case the cc sinks in)
        //public float startDistanceFromBottom = 0.2f; 
        public float groundCheckY = 0.33f; // 0.25 + 0.08 (sphereCastRadius + CC skin width)

        [Tooltip(
            "The position of the ceiling check sphere. Set to the height minus sphereCastRadius plus the CC Skin Width. Enable showGizmos to visualize.")]
        // this should extend above the cc (by approx skin width) so player can still move when not at full height (not crouching, trying to stand up), 
        // otherwise if it's below the top then the cc gets stuck
        public float ceilingCheckY = 1.83f; // 2.00 - 0.25 + 0.08 (height - sphereCastRadius + CC skin width) 

        [Space(5)] public float sphereCastRadius = 0.25f; // radius of area to detect for ground

        public float sphereCastDistance = 0.75f; // How far spherecast moves down from origin point

        [Space(5)] public float raycastLength = 0.75f; // secondary raycasts (match to sphereCastDistance)

        public Vector3 rayOriginOffset1 = new(-0.2f, 0f, 0.16f);
        public Vector3 rayOriginOffset2 = new(0.2f, 0f, -0.16f);

        [Header("Debug Gizmos")] [Tooltip("Show debug gizmos and lines")]
        public bool showGizmos; // Show debug gizmos and lines

        [Header("- reference variables -")] public float xRotation; // the up/down angle the player is looking

        [Space(5)] public bool isGrounded;

        public float groundSlopeAngle; // Angle of the slope in degrees
        public Vector3 groundSlopeDir = Vector3.zero; // The calculated slope as a vector
        public bool isSlipping;

        [Space(5)] public bool isSliding;

        public float slideTimer; // current slide duration
        public Vector3 slideForward = Vector3.zero; // direction of the slide

        [Space(5)] public bool isCeiling;

        [Space(5)] public bool cursorActive; // cursor state

        private float _accMouseX; // reference for mouse look smoothing
        private float _accMouseY; // reference for mouse look smoothing
        private float _cameraStartY; // reference to move camera with crouch
        private float _ceilingOffsetY; // calculated offset relative to height

        // - Components -
        private CharacterController _controller; // CharacterController component

        // - private reference variables -
        private float _defaultHeight; // reference to scale player crouch
        private Vector3 _fauxGravity = Vector3.zero; // calculated gravity
        private float _groundOffsetY; // calculated offset relative to height
        private Vector3 _lastPos = Vector3.zero; // reference for player velocity 
        private float _lastSpeed; // reference for calculating speed
        private Transform _playerTx; // this player object


        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            ProcessInputs();
            ProcessLook();
            ProcessMovement();
        }

        // this script pushes all rigidbodies that the character touches
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            var body = hit.collider.attachedRigidbody;

            // no rigidbody
            if (body == null || body.isKinematic) return;

            // We don't want to push objects below us
            if (hit.moveDirection.y < -0.3f) return;

            // If you know how fast your character is trying to move,
            // then you can also multiply the push velocity by that.
            body.linearVelocity = hit.moveDirection * _lastSpeed;
        }

        // Debug Gizmos
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (showGizmos)
            {
                if (!Application.isPlaying)
                {
                    _groundOffsetY = groundCheckY;
                    _ceilingOffsetY = ceilingCheckY;
                }

                var startPoint = new Vector3(transform.position.x, transform.position.y + _groundOffsetY,
                    transform.position.z);
                var endPoint = startPoint + new Vector3(0, -sphereCastDistance, 0);
                var ceilingPoint = new Vector3(transform.position.x, transform.position.y + _ceilingOffsetY,
                    transform.position.z);

                Gizmos.color = isGrounded ? Color.green : Color.white;
                Gizmos.DrawWireSphere(startPoint, sphereCastRadius);

                Gizmos.color = Color.gray;
                Gizmos.DrawWireSphere(endPoint, sphereCastRadius);

                Gizmos.DrawLine(startPoint, endPoint);

                Gizmos.color = isCeiling ? Color.red : Color.white;
                Gizmos.DrawWireSphere(ceilingPoint, sphereCastRadius);
            }
        }
#endif

        private void Initialize()
        {
            if (!cameraTx)
                Debug.LogError("* " + gameObject.name + ": BasicFPCC has NO CAMERA ASSIGNED in the Inspector *");

            _controller = GetComponent<CharacterController>();

            _playerTx = transform;
            _defaultHeight = _controller.height;
            _lastSpeed = 0;
            _fauxGravity = Vector3.up * gravity;
            _lastPos = _playerTx.position;
            _cameraStartY = cameraTx.localPosition.y;
            _groundOffsetY = groundCheckY;
            _ceilingOffsetY = ceilingCheckY;

            RefreshCursor();
        }

        private void ProcessInputs()
        {
            if (useLocalInputs)
            {
                inputLookX = Input.GetAxis(axisLookHorizontal);
                inputLookY = Input.GetAxis(axisLookVertical);

                inputMoveX = Input.GetAxis(axisMoveHorizontal);
                inputMoveY = Input.GetAxis(axisMoveVertical);

                inputKeyRun = Input.GetKey(keyRun);
                inputKeyCrouch = Input.GetKey(keyCrouch);

                inputKeyDownJump = Input.GetKeyDown(keyJump);
                inputKeyDownSlide = Input.GetKeyDown(keySlide);
                inputKeyDownCursor = Input.GetKeyDown(keyToggleCursor);
            }

            if (inputKeyDownCursor) ToggleLockCursor();
        }

        private void ProcessLook()
        {
            _accMouseX = Mathf.Lerp(_accMouseX, inputLookX, mouseSnappiness * Time.deltaTime);
            _accMouseY = Mathf.Lerp(_accMouseY, inputLookY, mouseSnappiness * Time.deltaTime);

            var mouseX = _accMouseX * mouseSensitivityX * 100f * Time.deltaTime;
            var mouseY = _accMouseY * mouseSensitivityY * 100f * Time.deltaTime;

            // rotate camera X
            xRotation += invertLookY ? mouseY : -mouseY;
            xRotation = Mathf.Clamp(xRotation, -clampLookY, clampLookY);

            cameraTx.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            // rotate player Y
            _playerTx.Rotate(Vector3.up * mouseX);
        }

        private void ProcessMovement()
        {
            // - variables -
            var vScale = 1f; // for calculating GFX scale (optional)
            var h = _defaultHeight;
            var nextSpeed = walkSpeed;
            Vector3 calc; // used for calculations
            Vector3 move; // direction calculation

            // player current speed
            var currSpeed = (_playerTx.position - _lastPos).magnitude / Time.deltaTime;
            currSpeed = currSpeed < 0 ? 0 - currSpeed : currSpeed; // abs value

            // - Check if Grounded -
            GroundCheck();

            isSlipping = groundSlopeAngle > _controller.slopeLimit;

            // - Check Ceiling above for Headroom -
            CeilingCheck();

            // - Run and Crouch -

            // if grounded, and not stuck on ceiling
            if (isGrounded && !isCeiling && inputKeyRun) nextSpeed = runSpeed; // to run speed

            if (inputKeyCrouch) // crouch
            {
                vScale = 0.5f;
                h = 0.5f * _defaultHeight;
                nextSpeed = crouchSpeed; // slow down when crouching
            }

            // - Slide -

            // if not sliding, and not stuck on ceiling, and is running
            if (!isSliding && !isCeiling && inputKeyRun && inputKeyDownSlide) // slide
                // check velocity is faster than walkSpeed
                if (currSpeed > walkSpeed)
                {
                    slideTimer = 0; // start slide timer
                    isSliding = true;
                    slideForward = (_playerTx.position - _lastPos).normalized;
                }

            _lastPos = _playerTx.position; // update reference

            // check slider timer and velocity
            if (isSliding)
            {
                nextSpeed = currSpeed; // default to current speed
                move = slideForward; // set input to direction of slide

                slideTimer += Time.deltaTime; // slide timer

                // if timer max, or isSliding and not moving, then stop sliding
                if (slideTimer > slideDuration || currSpeed < crouchSpeed)
                {
                    isSliding = false;
                }
                else // confirmed player is sliding
                {
                    vScale = 0.5f; // gfx scale
                    h = 0.5f * _defaultHeight; // height is crouch height
                    nextSpeed = slideSpeed; // to slide speed
                }
            }
            else // - Player Move Input -
            {
                move = _playerTx.right * inputMoveX + _playerTx.forward * inputMoveY;

                if (move.magnitude > 1f) move = move.normalized;
            }

            // - Height -

            // crouch/stand up smoothly
            var lastHeight = _controller.height;
            var nextHeight = Mathf.Lerp(_controller.height, h, 5f * Time.deltaTime);

            // if crouching, or only stand if there is no ceiling
            if (nextHeight < lastHeight || !isCeiling)
            {
                _controller.height = Mathf.Lerp(_controller.height, h, 5f * Time.deltaTime);

                // fix vertical position
                calc = _playerTx.position;
                calc.y += (_controller.height - lastHeight) / 2f;
                _playerTx.position = calc;

                // offset camera
                calc = cameraTx.localPosition;
                calc.y = _controller.height / _defaultHeight + _cameraStartY - _defaultHeight * 0.5f;
                cameraTx.localPosition = calc;

                // calculate offset
                var heightFactor = (_defaultHeight - _controller.height) * 0.5f;

                // offset ground check
                _groundOffsetY = heightFactor + groundCheckY;

                // offset ceiling check
                _ceilingOffsetY = heightFactor + _controller.height - (_defaultHeight - ceilingCheckY);

                // scale gfx (optional)
                if (playerGfx)
                {
                    calc = playerGfx.localScale;
                    calc.y = Mathf.Lerp(calc.y, vScale, 5f * Time.deltaTime);
                    playerGfx.localScale = calc;
                }
            }

            // - Slipping Jumping Gravity - 

            // smooth speed
            float speed;

            if (isGrounded)
            {
                if (isSlipping) // slip down slope
                {
                    // movement left/right while slipping down
                    // player rotation to slope
                    var slopeRight = Quaternion.LookRotation(Vector3.right) * groundSlopeDir;
                    var dot = Vector3.Dot(slopeRight, _playerTx.right);
                    // move on X axis, with Y rotation relative to slopeDir
                    move = slopeRight * (dot > 0 ? inputMoveX : -inputMoveX);

                    // speed
                    nextSpeed = Mathf.Lerp(currSpeed, runSpeed, 5f * Time.deltaTime);

                    // increase angular gravity
                    var mag = _fauxGravity.magnitude;
                    calc = Vector3.Slerp(_fauxGravity, groundSlopeDir * runSpeed, 4f * Time.deltaTime);
                    _fauxGravity = calc.normalized * mag;
                }
                else
                {
                    // reset angular fauxGravity movement
                    _fauxGravity.x = 0;
                    _fauxGravity.z = 0;

                    if (_fauxGravity.y < 0) // constant grounded gravity
                        //fauxGravity.y = -1f;
                        _fauxGravity.y = Mathf.Lerp(_fauxGravity.y, -1f, 4f * Time.deltaTime);
                }

                // - Jump -
                if (!isSliding && !isCeiling && inputKeyDownJump) // jump
                    _fauxGravity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

                // --

                // - smooth speed -
                // take less time to slow down, more time speed up
                var lerpFactor = _lastSpeed > nextSpeed ? 4f : 2f;
                speed = Mathf.Lerp(_lastSpeed, nextSpeed, lerpFactor * Time.deltaTime);
            }
            else // no friction, speed changes slower
            {
                speed = Mathf.Lerp(_lastSpeed, nextSpeed, 0.125f * Time.deltaTime);
            }

            // prevent floating if jumping into a ceiling
            if (isCeiling)
            {
                speed = crouchSpeed; // clamp speed to crouched

                if (_fauxGravity.y > 0) _fauxGravity.y = -1f; // 0;
            }

            _lastSpeed = speed; // update reference

            // - Add Gravity -

            _fauxGravity.y += gravity * Time.deltaTime;

            // - Move -

            calc = move * (speed * Time.deltaTime);
            calc += _fauxGravity * Time.deltaTime;

            _controller.Move(calc);

            // - DEBUG - 

#if UNITY_EDITOR
            // slope angle and fauxGravity debug info
            if (showGizmos)
            {
                calc = _playerTx.position;
                calc.y += _groundOffsetY;
                Debug.DrawRay(calc, groundSlopeDir.normalized * 5f, Color.blue);
                Debug.DrawRay(calc, _fauxGravity, Color.green);
            }
#endif
        }

        // lock/hide or show/unlock cursor
        public void SetLockCursor(bool doLock)
        {
            cursorActive = doLock;
            RefreshCursor();
        }

        private void ToggleLockCursor()
        {
            cursorActive = !cursorActive;
            RefreshCursor();
        }

        private void RefreshCursor()
        {
            if (!cursorActive && Cursor.lockState != CursorLockMode.Locked) Cursor.lockState = CursorLockMode.Locked;
            if (cursorActive && Cursor.lockState != CursorLockMode.None) Cursor.lockState = CursorLockMode.None;
        }

        // check the area above, for standing from crouch
        private void CeilingCheck()
        {
            var origin = new Vector3(_playerTx.position.x, _playerTx.position.y + _ceilingOffsetY,
                _playerTx.position.z);

            isCeiling = Physics.CheckSphere(origin, sphereCastRadius, castingMask);
        }

        // find if isGrounded, slope angle and directional vector
        private void GroundCheck()
        {
            //Vector3 origin = new Vector3( transform.position.x, transform.position.y - (controller.height / 2) + startDistanceFromBottom, transform.position.z );
            var origin = new Vector3(_playerTx.position.x, _playerTx.position.y + _groundOffsetY, _playerTx.position.z);

            // Out hit point from our cast(s)

            // SPHERECAST
            // "Casts a sphere along a ray and returns detailed information on what was hit."
            if (Physics.SphereCast(origin, sphereCastRadius, Vector3.down, out var hit, sphereCastDistance,
                    castingMask))
            {
                // Angle of our slope (between these two vectors). 
                // A hit normal is at a 90-degree angle from the surface that is collided with (at the point of collision).
                // e.g. On a flat surface, both vectors are facing straight up, so the angle is 0.
                groundSlopeAngle = Vector3.Angle(hit.normal, Vector3.up);

                // Find the vector that represents our slope as well. 
                //  temp: basically, finds vector moving across hit surface 
                var temp = Vector3.Cross(hit.normal, Vector3.down);
                //  Now use this vector and the hit normal, to find the other vector moving up and down the hit surface
                groundSlopeDir = Vector3.Cross(temp, hit.normal);

                // --
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            } // --

            // Now that's all fine and dandy, but on edges, corners, etc., we get angle values that we don't want.
            // To correct for this, let's do some raycasts. You could do more raycasts, and check for more
            // edge cases here. There are lots of situations that could pop up, so test and see what gives you trouble.

            // FIRST RAYCAST
            if (Physics.Raycast(origin + rayOriginOffset1, Vector3.down, out var slopeHit1, raycastLength))
            {
                // Debug line to first hit point
#if UNITY_EDITOR
                if (showGizmos) Debug.DrawLine(origin + rayOriginOffset1, slopeHit1.point, Color.red);
#endif
                // Get angle of slope on hit normal
                var angleOne = Vector3.Angle(slopeHit1.normal, Vector3.up);

                // 2ND RAYCAST
                if (Physics.Raycast(origin + rayOriginOffset2, Vector3.down, out var slopeHit2, raycastLength))
                {
                    // Debug line to second hit point
#if UNITY_EDITOR
                    if (showGizmos) Debug.DrawLine(origin + rayOriginOffset2, slopeHit2.point, Color.red);
#endif
                    // Get angle of slope of these two hit points.
                    var angleTwo = Vector3.Angle(slopeHit2.normal, Vector3.up);
                    // 3 collision points: Take the MEDIAN by sorting array and grabbing middle.
                    var tempArray = new[] { groundSlopeAngle, angleOne, angleTwo };
                    Array.Sort(tempArray);
                    groundSlopeAngle = tempArray[1];
                }
                else
                {
                    // 2 collision points (sphere and first raycast): AVERAGE the two
                    var average = (groundSlopeAngle + angleOne) / 2;
                    groundSlopeAngle = average;
                }
            }
        }
    }
}
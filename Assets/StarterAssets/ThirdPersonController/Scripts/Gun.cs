using UnityEngine;
using System.Collections;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace ThirdPerson
{
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    [RequireComponent(typeof(LineRenderer))]
#endif

    public class Gun : MonoBehaviour
    {


        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDShoots;
        private int _animIDAim;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _animIDSecondary;


        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;

        private bool _hasAnimator;

        public int gunDamage = 1;                                            // Set the number of hitpoints that this gun will take away from shot objects with a health script
        public float fireRate = 0.25f;                                        // Number in seconds which controls how often the player can fire
        public float weaponRange = 50f;                                        // Distance in Unity units over which the player can fire
        public float hitForce = 100f;                                        // Amount of force which will be added to objects with a rigidbody shot by the player
        public Transform gunEnd;                                            // Holds a reference to the gun end object, marking the muzzle location of the gun
        public RectTransform crosshair;
        public Camera fpsCam;                                                // Holds a reference to the first person camera
        private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);    // WaitForSeconds object used by our ShotEffect coroutine, determines time laser line will remain visible
        private AudioSource gunAudio;                                        // Reference to the audio source which will play our shooting sound effect
        private LineRenderer laserLine;                                        // Reference to the LineRenderer component which will display our laserline
        private float nextFire;
        public GameObject Player;
        private Vector3 TargetPoint;



        private void Start()
        {

           // crosshair = RectTransformUtility.ScreenPointToWorldPointInRectangle(crosshair, crosshair.position, fpsCam, out TargetPoint);
            _hasAnimator = Player.TryGetComponent(out _animator);
            _controller = Player.GetComponent<CharacterController>();
            _input = Player.GetComponent<StarterAssetsInputs>();
            AssignAnimationIDs();

            // Get and store a reference to our LineRenderer component
            laserLine = GetComponent<LineRenderer>();

            // Get and store a reference to our AudioSource component
            gunAudio = GetComponent<AudioSource>();

        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDShoots = Animator.StringToHash("Shoots");
            _animIDAim = Animator.StringToHash("Aim");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDSecondary = Animator.StringToHash("Secondary");
        }


        void Update()
        {
            // Check if the player has pressed the fire button and if enough time has elapsed since they last fired
            if (Time.time > nextFire &&
                 _input.fire && 
                _animator.GetBool(_animIDSecondary) == true)
            {
                // Update the time when our player can fire next
                nextFire = Time.time + fireRate;

                // Start our ShotEffect coroutine to turn our laser line on and off
                StartCoroutine(ShotEffect());

                // Create a vector at the center of our camera's viewport
                Ray rayOrigin = fpsCam.ScreenPointToRay(new Vector3(crosshair.position.x, crosshair.position.y, 0.0f));

                // Declare a raycast hit to store information about what our raycast has hit
                RaycastHit hit;

                // Set the start position for our visual effect for our laser to the position of gunEnd
                laserLine.SetPosition(0, gunEnd.position);

                // Check if our raycast has hit anything
                if (Physics.Raycast(rayOrigin, out hit, weaponRange))
                {
                    // Set the end position for our laser line 
                    laserLine.SetPosition(1, hit.point);

                    // Get a reference to a health script attached to the collider we hit
                    ///   ShootableBox health = hit.collider.GetComponent<ShootableBox>();

                    // If there was a health script attached
                    ///   if (health != null)
                    ///     {
                    // Call the damage function of that script, passing in our gunDamage variable
                    ///       health.Damage(gunDamage);
                    ///    }

                    // Check if the object we hit has a rigidbody attached
                    if (hit.rigidbody != null)
                    {
                        // Add force to the rigidbody we hit, in the direction from which it was hit
                        hit.rigidbody.AddForce(-hit.normal * hitForce);
                    }
                }
                else
                {
                    // If we did not hit anything, set the end of the line to a position directly in front of the camera at the distance of weaponRange
                    laserLine.SetPosition(1, rayOrigin.direction*weaponRange );
                }
            }
        }


        private IEnumerator ShotEffect()
        {
            // Play the shooting sound effect
            if(gunAudio != null)
                gunAudio.Play();

            // Turn on our line renderer
            laserLine.enabled = true;

            //Wait for .07 seconds
            yield return shotDuration;

            // Deactivate our line renderer after waiting
            laserLine.enabled = false;
        }
    }
}
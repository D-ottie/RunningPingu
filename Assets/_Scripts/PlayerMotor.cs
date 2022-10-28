using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private const float LANE_DISTANCE = 2.5f; // space between our lanes. 
    private const float TURN_SPEED = 0.3f;

    private CharacterController _characterController;
    private Animator _animator;

    private float jumpForce = 5f;
    private float gravity = 12f;
    private float verticalVelocity;
    private int desiredLane = 1; // 0 for Left, 1 for Middle and 2 for Right. 

    [SerializeField] // properties to modify speed as game goes on. 
    private float originalSpeed = 7.0f;
    private float speed;
    private float lastSpeedIncreaseTime;
    private float speedIncreaseTime = 2.5f;
    private float speedIncreaseAmt = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        speed = originalSpeed;
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // test if we have tapped the screen and the game had not started yet. 
        if (!GameManager.Instance.GameStarted)
            return;

        //if (MobileInput.Singleton.Tap && !GameManager.Instance.GameStarted)
        //{
        //    print("Not Touching UI");
        //    GameManager.Instance.gameMenu.SetTrigger("Show");
        //    StartRunning();
        //}

        if (Time.time - lastSpeedIncreaseTime > speedIncreaseTime)
        {
            // if the difference in our current time and the last time the speed increase is greater than the time we are to increase speed, 
            // then it's time to increase speed. 
            lastSpeedIncreaseTime = Time.time; // reset our last speed increase time. 
            speed += speedIncreaseAmt; // increase the speed by the amount we specified. 
            // modify the displayHUD. 
            GameManager.Instance.UpdateModifier(speed - originalSpeed);
        }

        // getting input. 
        if (MobileInput.Singleton.SwipeLeft)
            MoveLane(false);
        if (MobileInput.Singleton.SwipeRight)
            MoveLane(true);

        // calculating where the player should be after input. 
        Vector3 _targetPosition = transform.position.z * Vector3.forward;
        if (desiredLane == 0)
            _targetPosition += Vector3.left * LANE_DISTANCE;
        else if (desiredLane == 2)
            _targetPosition += Vector3.right * LANE_DISTANCE;

        // calculating the Move Delta. 
        Vector3 moveVector = Vector3.zero; // re-initialize. 

        /* 
         To simplify this delta very well, we take where we want to be minus where we are currently at, to get the 
        direction vector to follow to get where we want to be, then we normalize that vector to get a meter towards that distance
        and apply that speed now, which will then move us towards that distance with that speed per meter. Excellent stuff 
         */
        moveVector.x = (_targetPosition - transform.position).normalized.x * speed; // normalize the x direction. 


        bool _isGrounded = IsGrounded();
        _animator.SetBool("IsGrounded", _isGrounded);

        // Calculating Vertical movement - Y
        if (_isGrounded)
        {
            verticalVelocity = -0.1f; // make sure we always remain grounded
            // even if we were going down a slope or something similar (stairs) 
            if (MobileInput.Singleton.SwipeUp)
            {
                //jump
                _animator.SetTrigger("Jump");
                verticalVelocity = jumpForce;
            }
            else if (MobileInput.Singleton.SwipeDown)
            {
                // slide
                _animator.SetTrigger("Slide");
                _characterController.center = new Vector3(_characterController.center.x,
                    _characterController.center.y / 2,
                    _characterController.center.z); // modify the collider for slide. 
                _characterController.height /= 2;
                Invoke(nameof(StopSliding), 0.5f);
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime; // drop using our gravity per frame. 
            if (MobileInput.Singleton.SwipeDown)
            {
                // if jumping and we press pace, quickly descend. 
                verticalVelocity = -jumpForce;
            }
        }

        moveVector.y = verticalVelocity;
        moveVector.z = speed; // since we want to always move forward regardless of if we are switching lanes, sliding or jumping
        // our moveVector in the forward axis, has to always maintain our speed. 

        // Move the character according to the coordinates we defined above. 
        _characterController.Move(moveVector * Time.deltaTime);

        // lets rotate the character as it changes lanes
        Vector3 dir = _characterController.velocity;
        dir.y = 0;
        transform.forward = Vector3.Lerp(transform.forward, dir, TURN_SPEED);

        // update the score as we run. we could use events later for these things. 
    }

    private void StopSliding()
    {
        _animator.SetTrigger("Running");
        // we can also adjust these colliders using the Animation window. 
        _characterController.center = new Vector3(_characterController.center.x, _characterController.center.y * 2, _characterController.center.z);
        _characterController.height *= 2;
    }

    private void MoveLane(bool goingRight)
    {
        desiredLane += (goingRight) ? 1 : -1;
        desiredLane = Mathf.Clamp(desiredLane, 0, 2); // clamp lane movement within the 3 lanes.
    }

    private bool IsGrounded()
    {
        Ray groundRay = new Ray(new Vector3(
            _characterController.bounds.center.x,
            (_characterController.bounds.center.y - _characterController.bounds.extents.y) +
            0.2f, _characterController.bounds.center.z),
            Vector3.down); // ray needs an origin and a direction. The code above is just calculating these 2 parameters. 

        Debug.DrawRay(groundRay.origin, groundRay.direction, Color.magenta, 1f);

        // if the ray casted down using this distance hits something, it'll return true, otherwise false. 
        // we may have to check explicitly if it's hitting the ground later. 
        return Physics.Raycast(groundRay, 0.2f + 0.1f);
        // 0.1f is that skin variable we are estimating. 
    }

    public void StartRunning()
    {
        _animator.SetTrigger("Running");
        GameManager.Instance.gameMenu.SetTrigger("Show");
        GameManager.Instance.GameStarted = true;
    }

    // Character Controllers don't detect OnCollisionEnters. So we use OnControllerColliderHit for our COllision operations. 
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        switch (hit.gameObject.tag)
        {
            case "Obstacle":
                Crash();
                break;
            default:
                break;
        }
    }

    private void Crash()
    {
        GameManager.Instance.OnDeath();
        GameManager.Instance.GameStarted = false;
        _animator.SetTrigger("Death");
    }
}

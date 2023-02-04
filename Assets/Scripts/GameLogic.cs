using System.Collections;
using System.Collections.Generic;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public float health = 100f;
    public float enemy_damage = 5f; //to be referenced by enemy later (placeholder value)
    
    [Tooltip("Projectile that the player will shoot for his attack")]
    public Object projectile_prefab;
    
    [Tooltip("Amount of time between shots (in seconds)")]
    public float projectile_cooldown = 1f;

    public float projectile_spawnDistance = 3f;

    private Animator _animator;
    private CharacterController _controller;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    private PlayerInput _playerInput;
#endif

    private float lastShot = 0f;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if(!_animator.GetBool("Hurt"))
        {

            if (_playerInput.actions["Attack"].IsPressed() && Time.time - lastShot > projectile_cooldown)
            {
                StartCoroutine("Attack");
                lastShot = Time.time;
            }
        }
        
    }

    private IEnumerator Attack()
    {
        _animator.SetBool("Attacking", true);
        yield return new WaitForSeconds(0.5f);
        Vector3 spawnPoint = transform.position + transform.forward * projectile_spawnDistance;
        Instantiate(projectile_prefab, spawnPoint, transform.rotation);
        _animator.SetBool("Attacking", false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.tag == "Enemy")
        {
            Debug.Log("I got hit by enemy");
            _animator.SetBool("Hurt", true);
            health -= enemy_damage;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            _animator.SetBool("Hurt", false);
        }
    }

}

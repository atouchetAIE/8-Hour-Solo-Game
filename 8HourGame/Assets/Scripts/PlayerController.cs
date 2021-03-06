﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : Actor {

    public GameObject bullet;   //Player Bullet Prefab
    public int ammo;           //THe amount of bullets the player has.
    public float nextFire;
    private GameObject bullSpawnPoint;
    private GameObject cam;     //Camera
    private GameObject side;     //Camera
    private Vector3 movement;

    public int maxhealth;

    Slider helathSlider;
    Text bullets;

    void Lerp(Vector3 vec)    //Function for player movement
    {
        gameObject.transform.position = new Vector3(transform.position.x + vec.x, transform.position.y + vec.y, transform.position.z + vec.z);
    }

    void Start () {
        bullets = GameObject.Find("Bullets").GetComponent<Text>();     
        helathSlider = GameObject.Find("HelathSlider").GetComponent<Slider>();
        side = GameObject.Find("Side");
        cam = GameObject.Find("Main Camera");
        bullSpawnPoint = GameObject.Find("BSP");
        maxhealth = 100;
        health = 100;   //Player's Health: how much x before destroyed.
        speed = 25;      //Speed of the player. the higher it is the slower he will move
        bullDam = 10;   //How much damage the enemy will take.
        fireRate = 1f;  //How fast the player will be able to fire.
        ammo = 5;
        bullets.text = "Bullets: " + ammo.ToString();
	}	

    public void FixedUpdate()
    {
        //float z = Input.GetAxis("Horizontal");  //Get Left and right
        float rotate = Input.GetAxis("Look");
        rotate *= 1.5f;
        if (rotate != 0)   //Code used to rotate the player.
        {
            gameObject.transform.Rotate(new Vector3(0f, rotate, 0f));
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))   //If Button is pressed  
        {
            if (Input.GetKey(KeyCode.W))    //if W move forewards
            {
                movement = new Vector3(gameObject.transform.position.x - cam.gameObject.transform.position.x, 0, gameObject.transform.position.z - cam.gameObject.transform.position.z);
            }

            else if (Input.GetKey(KeyCode.S))   //if S, move back
            {
                movement = new Vector3(-(gameObject.transform.position.x - cam.gameObject.transform.position.x), 0, -(gameObject.transform.position.z - cam.gameObject.transform.position.z));
            }

            if (Input.GetKey(KeyCode.A))    //if A, strafe left.
            {
                movement = new Vector3(gameObject.transform.position.x - side.gameObject.transform.position.x, 0, gameObject.transform.position.z - side.gameObject.transform.position.z);
            }

            else if (Input.GetKey(KeyCode.D))   //if D, strafe right.
            {
                movement = new Vector3(-(gameObject.transform.position.x - side.gameObject.transform.position.x), 0, -(gameObject.transform.position.z - side.gameObject.transform.position.z));
            }
        }

        else
        {
            movement = new Vector3(0, 0, 0);        //None of the Buttons, stop moving
        }

        Lerp(movement / speed);         //Movement
    }

	override public void Update () {
        if((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E) ) && ammo > 0 && Time.time >= nextFire) //if Space bar is pressed down, ammo is greater than 0, and time is greater than or equal to time when you can fire again.
        {
            GameObject temp;    //Bullet that you just spawned based off a position.
            temp = Instantiate(bullet, bullSpawnPoint.transform.position, new Quaternion()) as GameObject;
            ammo -= 1;  //Subtract ammo
            temp.GetComponent<PlayerBulletControler>().damage = bullDam;    //Bullet's damage is equal to player's
            nextFire = fireRate + Time.time;    //Sets the next fire
            bullets.text = "Bullets: " + ammo.ToString();
        }
	}

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "EnemyBullet")  //If it's an enemy bullet
        {
            health -= other.gameObject.GetComponent<EnemyBulletController>().damage;   //Take damage
            helathSlider.value = health;
        }

        if (other.gameObject.tag == "Enemy")  //If it's an enemy
        {
            health -= other.gameObject.GetComponent<Actor>().bullDam;   //Take damage
            helathSlider.value = health;
        }

        if (other.gameObject.tag == "PlayerBullet") //if it is a player bullet
        {
            other.gameObject.GetComponent<PlayerBulletControler>().live = false; //And is dead
            ammo += 1;  //Add ammo
            Destroy(other.gameObject);  //Destroy that bullet
            bullets.text = "Bullets: " + ammo.ToString();
        }
    }
}

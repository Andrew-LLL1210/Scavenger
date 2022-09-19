using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // SceneManager
using System;
using UnityEngine.UI;

public class Player : MovingObject {
    public float restartLevelDelay = 1f; // in seconds
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public int wallDamage = 1;
    public Text foodText;
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;


    private Animator animator;
    private int food;


    protected override void Start() {
        animator = GetComponent<Animator>();
        food = GameManager.instance.playerFoodPoints;
        foodText.text = "Food: " + food;
        base.Start();
    }


    private void OnDisable() {
        GameManager.instance.playerFoodPoints = food;
    }


    private void Update() {
        if (!GameManager.instance.playersTurn) return;

        int horizontal = (int) Input.GetAxisRaw("Horizontal");
        int vertical = (int) Input.GetAxisRaw("Vertical");

        if (horizontal != 0) {
            vertical = 0;
        }

        if (horizontal != 0 || vertical != 0) {
            AttemptMove<Wall>(horizontal, vertical);
        }
    }

    protected override void AttemptMove<T>(int xDir, int yDir) { // T is Wall
        food--;

        foodText.text = "Food: " + food;

        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;
        if (Move(xDir, yDir, out hit)) {
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }

        CheckIfGameOver();

        GameManager.instance.playersTurn = false;
    }


    protected override void OnCantMove<T>(T component) { // T is Wall
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Exit") {
            Invoke(nameof(Restart), restartLevelDelay);

            enabled = false;
        }

        else if (other.tag == "Food") {
            food += pointsPerFood;

            foodText.text = "+" + pointsPerFood + " Food: " + food;

            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);

            other.gameObject.SetActive(false);
        }

        else if (other.tag == "Soda") {
            food += pointsPerSoda;

            foodText.text = "+" + pointsPerFood + " Food: " + food;

            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);

            other.gameObject.SetActive(false);
        }
    }

    private void Restart() {
        SceneManager.LoadScene(0);
    }

    // called by an enemy upon attacking the player
    // @param loss: how much food lost
    public void LoseFood(int loss) {
        animator.SetTrigger("playerHit");

        food -= loss;

        foodText.text = "-" + loss + " Food: " + food;

        CheckIfGameOver();
    }

    // ends the game if the player is out of food points
    private void CheckIfGameOver() {
        if (food <= 0) {
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            GameManager.instance.GameOver();
        }
    }
}

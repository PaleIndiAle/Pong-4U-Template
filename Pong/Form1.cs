/*
 * Description:     A basic PONG simulator
 * Author:           
 * Date:            
 */

#region libraries

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Media;
using System.Reflection;

#endregion

namespace Pong
{
    public partial class Form1 : Form
    {
        #region global values

        //graphics objects for drawing
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        Font drawFont = new Font("Courier New", 10);

        // Sounds for game
        SoundPlayer scoreSound = new SoundPlayer(Properties.Resources.score);
        SoundPlayer collisionSound = new SoundPlayer(Properties.Resources.collision);
        SoundPlayer paddleHitSound = new SoundPlayer(Properties.Resources.paddle);

        // check to see if a new game can be started
        Boolean newGameOk = true;

        //ball values
        Boolean ballMoveRight = true;
        Boolean ballMoveDown = true;
        int PLAYER_SPEED = 5;
        int BALL_SPEED = 4;
        const int BALL_WIDTH = 20;
        const int BALL_HEIGHT = 20; 
        Rectangle ball;

        //player values
        const int PADDLE_SPEED = 4;
        const int PADDLE_EDGE = 20;  // buffer distance between screen edge and paddle            
        const int PADDLE_WIDTH = 10;
        const int PADDLE_HEIGHT = 40;
        Rectangle player1, player2;

        //player and game scores
        int player1Score = 0;
        int player2Score = 0;
        int gameWinScore = 2;  // number of points needed to win game

        bool wPressed = false;
        bool sPressed = false;
        bool upPressed = false;
        bool downPressed = false;

        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //check to see if a key is pressed and set is KeyDown value to true if it has
            switch (e.KeyCode)
            {
                case Keys.W:
                    wPressed = true;
                    break;
                case Keys.S:
                    sPressed = true;
                    break;
                case Keys.Up:
                    upPressed = true;
                    break;
                case Keys.Down:
                    downPressed = true;
                    break;
                case Keys.Space:
                    if (newGameOk)
                    {
                        InitializeGame();
                    }
                    break;
                case Keys.Escape:
                    if (newGameOk)
                    {
                        Close();
                    }
                    break;
            }
        }
        
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //check to see if a key has been released and set its KeyDown value to false if it has
            switch (e.KeyCode)
            {
                case Keys.W:
                    wPressed = false;
                    break;
                case Keys.S:
                    sPressed = false;
                    break;
                case Keys.Up:
                    upPressed = false;
                    break;
                case Keys.Down:
                    downPressed = false;
                    break;
            }
        }

        /// <summary>
        /// sets the ball and paddle positions for game start
        /// </summary>
        private void InitializeGame()
        {
            if (newGameOk)
            {
                player1Score = 0;
                player2Score = 0;
                newGameOk = false;
                startLabel.Visible = false;
                gameUpdateLoop.Start();
                plaery2ScoreLabel.Text = $"{player2Score}";
                player1ScoreLabel.Text = $"{player1Score}";
            }

            //player start positions
            player1 = new Rectangle(PADDLE_EDGE, this.Height / 2 - PADDLE_HEIGHT / 2, PADDLE_WIDTH, PADDLE_HEIGHT);
            player2 = new Rectangle(this.Width - PADDLE_EDGE - PADDLE_WIDTH, this.Height / 2 - PADDLE_HEIGHT / 2, PADDLE_WIDTH, PADDLE_HEIGHT);

            // TODO create a ball rectangle in the middle of screen
            ball = new Rectangle(this.Width / 2 - 20, this.Height / 2 - 20, 20, 20);
        }

        /// <summary>
        /// This method is the game engine loop that updates the position of all elements
        /// and checks for collisions.
        /// </summary>
        private void gameUpdateLoop_Tick(object sender, EventArgs e)
        {
            #region update ball position

            // TODO create code to move ball either left or right based on ballMoveRight and using BALL_SPEED
            if (ballMoveRight == true)
            {
                ball.X = ball.X + BALL_SPEED;
            }
            else if (ballMoveRight == false)
            {
                ball.X = ball.X - BALL_SPEED;
            }

            // TODO create code move ball either down or up based on ballMoveDown and using BALL_SPEED
            if (ballMoveDown == true)
            {
                ball.Y = ball.Y + BALL_SPEED;
            }
            else if (ballMoveDown == false)
            {
                ball.Y = ball.Y - BALL_SPEED;
            }

            #endregion

            #region update paddle positions

            if (wPressed == true && player1.Y > 0)
            {
                player1.Y = player1.Y - PLAYER_SPEED;
            }

            // TODO create an if statement and code to move player 1 down 
            if (sPressed == true && player1.Y < this.Height - player1.Height)
            {
                player1.Y = player1.Y + PLAYER_SPEED;
            }

            // TODO create an if statement and code to move player 2 up
            if (upPressed == true && player2.Y > 0)
            {
                player2.Y = player2.Y - PLAYER_SPEED;
            }

            // TODO create an if statement and code to move player 2 down
            if (downPressed == true && player2.Y < this.Height - player2.Height)
            {
                player2.Y = player2.Y + PLAYER_SPEED;
            }

            #endregion

            #region ball collision with top and bottom lines

            if (ball.Y < 0) // if ball hits top line
            {
                // TODO use ballMoveDown boolean to change direction
                ballMoveDown = true;
                // TODO play a collision sound
                collisionSound.Play();
            } 
            else if (ball.Y > this.Height - 20) 
            {
                // Move up
                ballMoveDown = false;
                // collision sound
                collisionSound.Play();
            }

            #endregion

            #region ball collision with paddles

            // TODO create if statment that checks if player1 collides with ball and if it does
            // --- play a "paddle hit" sound and
            // --- use ballMoveRight boolean to change direction
            if (player1.IntersectsWith(ball))
            {
                paddleHitSound.Play();
                ballMoveRight = true;
                BALL_SPEED = BALL_SPEED + 2;
                PLAYER_SPEED = PLAYER_SPEED + 1;
            }
            else if (player2.IntersectsWith(ball))
            {
                paddleHitSound.Play();
                ballMoveRight = false;
                BALL_SPEED = BALL_SPEED + 2;
                PLAYER_SPEED = PLAYER_SPEED + 1;
            }

            #endregion

            #region ball collision with side walls (point scored)

            if (ball.X < 0)  // ball hits left wall logic
            {
                // TODO
                // --- play score sound
                // --- update player 2 score and display it to the label
                scoreSound.Play();
                player2Score++;
                plaery2ScoreLabel.Text = $"{player2Score}";

                if (player2Score == gameWinScore)
                {
                    GameOver("Player 2");
                }
                else
                {
                    BALL_SPEED = 4;
                    PLAYER_SPEED = 5;
                    ball.X = this.Width / 2 - 20;
                    ball.Y = this.Height / 2 - 20;
                    ballMoveRight = true;
                }

                // TODO use if statement to check to see if player 2 has won the game. If true run 
                // GameOver() method. Else change direction of ball and call SetParameters() method.

            }

            // TODO same as above but this time check for collision with the right wall
            if (ball.X > this.Width - BALL_WIDTH)
            {
                scoreSound.Play();
                player1Score++;
                player1ScoreLabel.Text = $"{player1Score}";

                if (player1Score == gameWinScore)
                {
                    GameOver("Player 1");
                }
                else
                {
                    BALL_SPEED = 4;
                    PLAYER_SPEED = 5;
                    ball.X = this.Width / 2 - 20;
                    ball.Y = this.Height / 2 - 20;
                    ballMoveRight = false;
                }
            }

            #endregion

            //refresh the screen, which causes the Form1_Paint method to run
            this.Refresh();
        }
        
        /// <summary>
        /// Displays a message for the winner when the game is over and allows the user to either select
        /// to play again or end the program
        /// </summary>
        /// <param name="winner">The player name to be shown as the winner</param>
        private void GameOver(string winner)
        {
            newGameOk = true;

            // TODO create game over logic
            // --- stop the gameUpdateLoop
            // --- show a message on the startLabel to indicate a winner, (may need to Refresh).
            // --- use the startLabel to ask the user if they want to play again
            gameUpdateLoop.Stop();
            startLabel.Text = $"{winner} Won! \n Press Space to play again.";
            startLabel.Visible = true;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // TODO draw player2 using FillRectangle
            e.Graphics.FillRectangle(whiteBrush, player1);
            e.Graphics.FillRectangle(whiteBrush, player2);

            // TODO draw ball using FillRectangle
            e.Graphics.FillRectangle(whiteBrush, ball);
        }

    }
}

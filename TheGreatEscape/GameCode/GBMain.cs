using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using PlatformGame;

namespace TheGreatEscape
{
    /// <summary>
    /// This is the main type of the Project
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        enum GameState
        {
            Start,
            Menu,
            Level1,
            Level2,
            Level3,
            Loss,
            Win
        }
        private Brick tempBrick;
        private static SpriteFont UiFont;
        private Texture2D arrowmin, arrowmax, circle;
        private bool powerupActive;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private GameState currentGameState;
        private KeyboardState keyboardState, oldKeyboardState;
        private GamePadState currPad, oldPad;
        private Random RNG;
        private Menu mainmenu;
        private Texture2D menuImage, tutorialImage;
        private End endScreen;
        private Win winScreen;
        private bool started;
        private SoundEffect ballSound, brickSound, glassBrickSound, cementBrickSound, powerBrickSound, ballBottom;
        private Song menuMusic, level1Music, level2Music, level3Music, winMusic, LossMusic;  
        private Background background;
        private Noise noise;
        private Bar playerBar;
        private Ball ball, secondBall;
        private Claustrophobia claustrophobiaBar;
        private float claustrophobiaPoints, speedHit, powerUpTimer;
        private int noisePoints;
        private int ballSpeed = 4;
        private int bricksWideLevel1 = 15;
        private int brickshighlevel1 = 4;
        private int bricksWideLevel2 = 15;
        private int brickshighLevel2 = 5;
        private int bricksWideLevel3 = 15;
        private int bricksHighLevel3 = 6;
        private int m_score, brickNumber;
        private int deadBricks;
        private int remainingBricks;
        private Texture2D brickImage, glassBrickImage, cementBrickImage, powerBrickImage;
        private Brick[,] bricks;
        private readonly Rectangle GameScreenRectangle;
        private bool songStarted;
       

        
        /// <summary>
        /// Main Constructor
        /// </summary>
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            graphics.IsFullScreen = false;
            GameScreenRectangle = new Rectangle(0,0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            Content.RootDirectory = "Content";
            currentGameState = GameState.Menu;
            RNG = new Random();
        }
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            MediaPlayer.IsRepeating = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            
            tempBrick = new BrickNormal(Vector2.Zero, brickImage, Color.White, brickSound, 1);
            tutorialImage = Content.Load<Texture2D>("Textures\\tutorial");
            menuImage = Content.Load<Texture2D>("Textures\\menu");
            UiFont = Content.Load<SpriteFont>("Fonts\\uiFont");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            arrowmin = Content.Load<Texture2D>("Textures\\bb_min");
            arrowmax = Content.Load<Texture2D>("Textures\\bb_max");
            circle = Content.Load<Texture2D>("Textures\\circle");
            noise = new Noise(new Vector2(70, 700), Content.Load<Texture2D>("Textures\\noise"));
            mainmenu = new Menu(Vector2.Zero, menuImage, tutorialImage);
            endScreen = new End(Vector2.Zero, Content.Load<Texture2D>("Textures\\end"));
            winScreen = new Win(Vector2.Zero, Content.Load<Texture2D>("Textures\\win"));
            playerBar = new Bar(Vector2.Zero, Content.Load<Texture2D>("Textures\\bar"), GameScreenRectangle);
            ballSound = Content.Load<SoundEffect>("Sound\\hit");
            brickSound = Content.Load<SoundEffect>("Sound\\brick");
            glassBrickSound = Content.Load<SoundEffect>("Sound\\glass");
            cementBrickSound = Content.Load<SoundEffect>("Sound\\metal");
            powerBrickSound = Content.Load<SoundEffect>("Sound\\power");
            ball = new Ball(Vector2.Zero, Content.Load<Texture2D>("Textures\\ball"), GameScreenRectangle, ballSound);
            secondBall = new Ball(Vector2.Zero, Content.Load<Texture2D>("Textures\\ball"), GameScreenRectangle,ballSound);
            brickImage = Content.Load<Texture2D>("Textures\\brick");
            glassBrickImage = Content.Load<Texture2D>("Textures\\glassbrick");
            cementBrickImage = Content.Load<Texture2D>("Textures\\cementBrick");
            powerBrickImage = Content.Load<Texture2D>("Textures\\powerbrick");
            background = new Background(Vector2.Zero, Content.Load<Texture2D>("Textures\\game"));
            claustrophobiaBar = new Claustrophobia(Vector2.One, Content.Load<Texture2D>("Textures\\barred"),claustrophobiaPoints);
            menuMusic = Content.Load<Song>("Sound\\menu");
            level1Music = Content.Load<Song>("Sound\\Level1");
            level2Music = Content.Load<Song>("Sound\\Level1");
            level3Music = Content.Load<Song>("Sound\\Level1");
            winMusic = Content.Load<Song>("Sound\\win");
            ballBottom = Content.Load<SoundEffect>("Sound\\fail");
            LossMusic = Content.Load<Song>("Sound\\loss");
            StartLevel(0);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();
            currPad = GamePad.GetState(PlayerIndex.One);
#region DEBUG
#if DEBUG
            if (keyboardState.IsKeyDown(Keys.W))
                claustrophobiaPoints++;
            if (keyboardState.IsKeyDown(Keys.S))
                claustrophobiaPoints--;
            if (keyboardState.IsKeyDown(Keys.X) && oldKeyboardState.IsKeyUp(Keys.X))
            {
                currentGameState = GameState.Level1;
                StartLevel(0);
            }
            if (keyboardState.IsKeyDown(Keys.C) && oldKeyboardState.IsKeyUp(Keys.C))
            {
                currentGameState = GameState.Level2;
                StartLevel(1);
            }
            if (keyboardState.IsKeyDown(Keys.V) && oldKeyboardState.IsKeyUp(Keys.V))
            {
                currentGameState = GameState.Level3;
                StartLevel(2);
            }
#endif
#endregion
            
            //Determines the current game state and calls the apropriate update
            switch (currentGameState)
            {
#region START UPDATE
                case GameState.Start:
                    break;
#endregion
#region MENU UPDATE
                case GameState.Menu:
                    mainmenu.update(keyboardState, oldKeyboardState);
                    if (!songStarted)
                    {
                        MediaPlayer.Play(menuMusic);
                        songStarted = true;
                    }
                    if (keyboardState.IsKeyDown(Keys.Enter) && oldKeyboardState.IsKeyDown(Keys.Enter))
                    {
                        StartLevel(0);
                        currentGameState = GameState.Level1;
                    }
                    if (keyboardState.IsKeyDown(Keys.Space) && oldKeyboardState.IsKeyDown(Keys.Space))
                    {
                       this.Exit();
                    }
                    break;
#endregion
#region LEVEL 1 UPDATE
                case GameState.Level1:
                    playerBar.Update(keyboardState, currPad, oldPad, GameScreenRectangle);
                    ball.Update(ballSpeed);
                   
                    ball.BarCollision(playerBar.GetBoundingBox(), speedHit, playerBar);
                    if (!songStarted)
                    {
                        MediaPlayer.Play(level1Music);
                        songStarted = true;
                    }
                    if (powerUpTimer > 0)
                    {
                        powerUpTimer -= 0.1f;
                        powerupActive = true;
                    }
                    else
                    {
                        powerUpTimer = 0;
                        powerupActive = false;
                    }
                    if (powerupActive)
                    {
                        secondBall.Update(ballSpeed);
                        secondBall.BarCollision(playerBar.GetBoundingBox(), speedHit, playerBar); 
                    }
                    noise.Update(noisePoints);
                    foreach (var brick in bricks)
                    {
                        brick.Update(gameTime, claustrophobiaPoints, m_score, brickNumber);
                        if (powerupActive)
                        {
                            if (brick.Collided(secondBall))
                                brick.OnCollide(secondBall);
                        }
                        if (brick.Collided(ball))
                        {
                            brick.OnCollide(ball);
                            powerUpTimer += brick.AssignPowerUp();
                            if (powerUpTimer == 120 && !powerupActive)
                            {
                                secondBall.SetPosition(brick.ReturnPosition());
                                secondBall.SetX(RNG.Next(-1, 1));
                            }
                        }
                        claustrophobiaPoints = brick.AssignPoints();
                        m_score = brick.AssignScore();
                        brickNumber = brick.BricksKilled();
                    }
                    if (!started)
                    {
                        powerUpTimer = 0;
                        ball.SetInStartPosition(playerBar.GetBounds());
                    }
                    if (keyboardState.IsKeyDown(Keys.Space) && !started)
                    {
                        ball.ReleaseBall();
                        started = true;
                    }
                    deadBricks = 0;
                     for (var y = 0; y < brickshighlevel1; y++)
                         for (int x = 0; x < bricksWideLevel1; x++)
                            if(bricks[x, y].CheckCondition())
                                 deadBricks += 1;

                     if (deadBricks == bricks.Length)
                     {
                         deadBricks = 0;
                         StartLevel(1);
                         currentGameState = GameState.Level2;
                         songStarted = false;
                     }
                    remainingBricks = bricks.Length - deadBricks;
                    claustrophobiaBar.Update(claustrophobiaPoints);
                    if (ball.OffBottom())
                    {
                        started = false;
                        playerBar.SetInStartPosition();
                        noisePoints -= 1;
                        ballBottom.Play();
                    }
                    if (secondBall.OffBottom())
                    {
                        secondBall.Deflection(tempBrick);
                    }
                    if (noisePoints <= 0)
                    {
                    currentGameState = GameState.Loss;
                    songStarted = false;
                    }
                    if(started)
                        claustrophobiaPoints += 0.1f;
                    if (claustrophobiaPoints >= 499)
                    {
                        currentGameState = GameState.Loss;
                        songStarted = false;
                    }
                    if (claustrophobiaPoints < 0)
                        claustrophobiaPoints = 0;
                    break;
#endregion
#region LEVEL 2  UPDATE
                  case GameState.Level2:
                      
                    playerBar.Update(keyboardState, currPad, oldPad, GameScreenRectangle);
                    ball.Update(ballSpeed);
                    ball.BarCollision(playerBar.GetBoundingBox(), speedHit, playerBar);
                    if (!songStarted)
                    {
                        MediaPlayer.Play(level1Music);
                        songStarted = true;
                    }
                    if (powerUpTimer > 0)
                    {
                        powerUpTimer -= 0.1f;
                        powerupActive = true;
                        
                    }
                    else
                    {
                        powerUpTimer = 0;
                        powerupActive = false;
                        
                    }
                    if (powerupActive)
                    {
                        secondBall.Update(ballSpeed);
                        secondBall.BarCollision(playerBar.GetBoundingBox(), speedHit, playerBar); 
                    }
                    noise.Update(noisePoints);
                    foreach (Brick brick in bricks)
                    {
                        brick.Update(gameTime, claustrophobiaPoints, m_score, brickNumber);
                        if (powerupActive)
                        {
                            if (brick.Collided(secondBall))
                                brick.OnCollide(secondBall);
                        }
                        if (brick.Collided(ball))
                        {
                            brick.OnCollide(ball);
                            powerUpTimer += brick.AssignPowerUp();
                            if (powerUpTimer == 120 && !powerupActive)
                            {
                                secondBall.SetPosition(brick.ReturnPosition());
                                secondBall.SetX(RNG.Next(-1, 1));
                            }
                            

                        }
                        claustrophobiaPoints = brick.AssignPoints();
                        m_score = brick.AssignScore();
                        brickNumber = brick.BricksKilled();
                    }
                    if (!started)
                    {
                        powerUpTimer = 0;
                        ball.SetInStartPosition(playerBar.GetBounds());
                    }
                    if (keyboardState.IsKeyDown(Keys.Space) && !started)
                    {
                        ball.ReleaseBall();
                        started = true;
                    }
                    deadBricks = 0;
                     for (var y = 0; y < brickshighLevel2; y++)
                         for (int x = 0; x < bricksWideLevel2; x++)
                            if(bricks[x, y].CheckCondition())
                                 deadBricks += 1;

                     if (deadBricks == bricks.Length)
                     {
                         deadBricks = 0;
                         StartLevel(2);
                         currentGameState = GameState.Level3;
                         songStarted = false;
                     }
                    remainingBricks = bricks.Length - deadBricks;
                    claustrophobiaBar.Update(claustrophobiaPoints);
                    if (ball.OffBottom())
                    {
                        started = false;
                        playerBar.SetInStartPosition();
                        noisePoints -= 1;
                        ballBottom.Play();
                    }

                    if (secondBall.OffBottom())
                    {
                        secondBall.Deflection(tempBrick);

                    }
                    if (noisePoints <= 0)
                    {
                    currentGameState = GameState.Loss;
                    songStarted = false;
                    }
                    if(started)
                        claustrophobiaPoints += 0.1f;
                    if (claustrophobiaPoints >= 499)
                    {
                        currentGameState = GameState.Loss;
                        songStarted = false;
                    }
                    if (claustrophobiaPoints < 0)
                        claustrophobiaPoints = 0;
                   
                    break;
                #endregion
#region LEVEL 3 UPDATE
               case GameState.Level3:
                       
                    playerBar.Update(keyboardState, currPad, oldPad, GameScreenRectangle);
                    ball.Update(ballSpeed);
                    ball.BarCollision(playerBar.GetBoundingBox(), speedHit, playerBar);
                    if (!songStarted)
                    {
                        MediaPlayer.Play(level1Music);
                        songStarted = true;
                    }
                    if (powerUpTimer > 0)
                    {
                        powerUpTimer -= 0.1f;
                        powerupActive = true;
                         
                    }
                    else
                    {
                        powerUpTimer = 0;
                        powerupActive = false;
                       
                    }
                    if (powerupActive)
                    {
                        secondBall.Update(ballSpeed);
                        secondBall.BarCollision(playerBar.GetBoundingBox(), speedHit, playerBar); 
                    }
                    noise.Update(noisePoints);
                    foreach (Brick brick in bricks)
                    {
                        brick.Update(gameTime, claustrophobiaPoints, m_score, brickNumber);
                        if (powerupActive)
                        {
                            if (brick.Collided(secondBall))
                                brick.OnCollide(secondBall);
                        }
                        if (brick.Collided(ball))
                        {
                            brick.OnCollide(ball);
                            powerUpTimer += brick.AssignPowerUp();
                            if (powerUpTimer == 120 && !powerupActive)
                            {
                                secondBall.SetPosition(brick.ReturnPosition());
                                secondBall.SetX(RNG.Next(-1, 1));
                            }
                            

                        }
                        claustrophobiaPoints = brick.AssignPoints();
                        m_score = brick.AssignScore();
                        brickNumber = brick.BricksKilled();
                    }
                    if (!started)
                    {
                        powerUpTimer = 0;
                        ball.SetInStartPosition(playerBar.GetBounds());
                    }
                    if (keyboardState.IsKeyDown(Keys.Space) && !started)
                    {
                        ball.ReleaseBall();
                        started = true;
                    }
                    deadBricks = 0;
                     for (var y = 0; y < bricksHighLevel3; y++)
                         for (int x = 0; x < bricksWideLevel3; x++)
                            if(bricks[x, y].CheckCondition())
                                 deadBricks += 1;

                     if (deadBricks == bricks.Length)
                     {
                         deadBricks = 0;
                         StartLevel(1);
                         currentGameState = GameState.Win;
                         songStarted = false;
                     }
                    remainingBricks = bricks.Length - deadBricks;
                    claustrophobiaBar.Update(claustrophobiaPoints);
                    if (ball.OffBottom())
                    {
                        started = false;
                        playerBar.SetInStartPosition();
                        noisePoints -= 1;
                        ballBottom.Play();
                    }

                    if (secondBall.OffBottom())
                    {
                        secondBall.Deflection(tempBrick);

                    }
                    if (noisePoints <= 0)
                    {
                    currentGameState = GameState.Loss;
                    songStarted = false;
                    }
                    if(started)
                        claustrophobiaPoints += 0.1f;
                    if (claustrophobiaPoints >= 499)
                    {
                        currentGameState = GameState.Loss;
                        songStarted = false;
                    }
                    if (claustrophobiaPoints < 0)
                        claustrophobiaPoints = 0;
                   
                    break;
#endregion
#region LOSS UPDATE
                    
                case GameState.Loss:
                    
                    if (!songStarted)
                    {
                        MediaPlayer.Play(LossMusic);
                        songStarted = true;
                    }
                    if (keyboardState.IsKeyDown(Keys.Escape) && oldKeyboardState.IsKeyDown(Keys.Escape))
                    {
                        songStarted = false;
                        currentGameState = GameState.Menu;
                    }
                    break;
#endregion
#region WIN UPDATE
                case GameState.Win:
                    
                    
                    if (!songStarted)
                    {
                        MediaPlayer.Play(winMusic);
                        songStarted = true;
                    }
                    if (keyboardState.IsKeyDown(Keys.Escape) && oldKeyboardState.IsKeyDown(Keys.Escape))
                    {
                        songStarted = false;
                        currentGameState = GameState.Menu;
                    }
                    break;
#endregion
                default:
                    throw new ArgumentOutOfRangeException();
            }
            oldPad = currPad;
            oldKeyboardState = keyboardState;
            base.Update(gameTime);
        }

        /// <summary>
        /// Depending on the inputed integer a different level is initialised. Everthing but the Score is reset.
        /// </summary>
        private void StartLevel(int level)
        {
            powerUpTimer = 0;
            MediaPlayer.Stop();
            songStarted = false;
            started = false;
            noisePoints = 3;
            switch (level)
            {
#region STARTLEVEL1
                case 0:
                    m_score = 0;
                    powerUpTimer = 0;
                    playerBar.SetInStartPosition();
                    ball.SetInStartPosition(playerBar.GetBounds());
                    bricks = new Brick[bricksWideLevel1, brickshighlevel1];
                    claustrophobiaPoints = 0;
                    speedHit = 0.2f;
                    deadBricks = 0;
                    for (var y = 0; y < brickshighlevel1; y++)
                    {
                        var tint = Color.White;
                        switch (y)
                        {
                            case 0:
                                tint = Color.White;
                                break;
                            case 1:
                                tint = Color.Wheat;
                                break;
                            case 2:
                                tint = Color.White;
                                break;
                            case 3:
                                tint = Color.Wheat;
                                break;
                            case 4:
                                tint = Color.White;
                                break;
                            case 5:
                                tint = Color.Wheat;
                                break;
                            case 6:
                                tint = Color.White;
                                break;
                        }
                        for (int x = 0; x < bricksWideLevel1; x++)
                        {
                            int i = RNG.Next(0, 24);
                            if(i <= 15)
                                bricks[x, y] = new BrickNormal(new Vector2(brickImage.Width * (1.4f * x) + 295, brickImage.Height * (1.4f * y) + 20), brickImage, tint, brickSound, 0);
                            if (i >= 16 && i <= 19 )
                                bricks[x, y] = new GlassBrick(new Vector2(brickImage.Width * (1.4f * x) + 295, brickImage.Height * (1.4f * y) + 20), glassBrickImage, tint, glassBrickSound, 0);
                            if (i == 20 || i == 21 )
                                bricks[x, y] = new MetalBrick(new Vector2(brickImage.Width * (1.4f * x) + 295, brickImage.Height * (1.4f * y) + 20), cementBrickImage, tint, cementBrickSound, 0);
                            if (i == 22 || i ==23)
                                bricks[x, y] = new PowerBrick(new Vector2(brickImage.Width * (1.4f * x) + 295, brickImage.Height * (1.4f * y) + 20), powerBrickImage, tint, powerBrickSound, 0);
                        }
                    }
                    break;
#endregion
#region STARTLEVEL2
                case 1:
                    powerUpTimer = 0;
                    playerBar.SetInStartPosition();
                    ball.SetInStartPosition(playerBar.GetBounds());
                    bricks = new Brick[bricksWideLevel2, brickshighLevel2];
                    claustrophobiaPoints = 0;
                    speedHit = 0.3f;
                    for (var y = 0; y < brickshighLevel2; y++)
                    {
                        var tint = Color.White;
                        switch (y)
                        {
                            case 0:
                                tint = Color.White;
                                break;
                            case 1:
                                tint = Color.Wheat;
                                break;
                            case 2:
                                tint = Color.White;
                                break;
                            case 3:
                                tint = Color.Wheat;
                                break;
                            case 4:
                                tint = Color.White;
                                break;
                            case 5:
                                tint = Color.Wheat;
                                break;
                            case 6:
                                tint = Color.White;
                                break;
                        }
                        for (int x = 0; x < bricksWideLevel2; x++)
                        {
                            int i = RNG.Next(0, 23);
                            if(i <= 15)
                                bricks[x, y] = new BrickNormal(new Vector2(brickImage.Width * (1.4f * x) + 295, brickImage.Height * (1.4f * y) + 20), brickImage, tint, brickSound, 0);
                            if (i >= 16 && i  <= 18)
                                bricks[x, y] = new GlassBrick(new Vector2(brickImage.Width * (1.4f * x) + 295, brickImage.Height * (1.4f * y) + 20), glassBrickImage, tint, glassBrickSound, 0);
                            if (i == 19 || i == 20 || i == 21 )
                                bricks[x, y] = new MetalBrick(new Vector2(brickImage.Width * (1.4f * x) + 295, brickImage.Height * (1.4f * y) + 20), cementBrickImage, tint, cementBrickSound, 0);
                            if (i == 22)
                                bricks[x, y] = new PowerBrick(new Vector2(brickImage.Width * (1.4f * x) + 295, brickImage.Height * (1.4f * y) + 20), powerBrickImage, tint, powerBrickSound, 0);
                        }
                    }
                    break;
#endregion
#region STARTLEVEL3
                case 2:
                    powerUpTimer = 0;
                    playerBar.SetInStartPosition();
                    ball.SetInStartPosition(playerBar.GetBounds());
                    bricks = new Brick[bricksWideLevel3, bricksHighLevel3];
                    claustrophobiaPoints = 0;
                    speedHit = 0.5f;
                    for (var y = 0; y < bricksHighLevel3; y++)
                    {
                        var tint = Color.White;
                        switch (y)
                        {
                            case 0:
                                tint = Color.White;
                                break;
                            case 1:
                                tint = Color.Wheat;
                                break;
                            case 2:
                                tint = Color.White;
                                break;
                            case 3:
                                tint = Color.Wheat;
                                break;
                            case 4:
                                tint = Color.White;
                                break;
                            case 5:
                                tint = Color.Wheat;
                                break;
                            case 6:
                                tint = Color.White;
                                break;
                        }
                        for (int x = 0; x < bricksWideLevel3; x++)
                        {
                            int i = RNG.Next(0, 23);
                            if(i <= 14)
                                bricks[x, y] = new BrickNormal(new Vector2(brickImage.Width * (1.4f * x) + 295, brickImage.Height * (1.4f * y) + 20), brickImage, tint, brickSound, 0);
                            if (i >= 15 && i  <= 17)
                                bricks[x, y] = new GlassBrick(new Vector2(brickImage.Width * (1.4f * x) + 295, brickImage.Height * (1.4f * y) + 20), glassBrickImage, tint, glassBrickSound, 0);
                            if (i == 18 || i == 19 || i == 20 || i == 21)
                                bricks[x, y] = new MetalBrick(new Vector2(brickImage.Width * (1.4f * x) + 295, brickImage.Height * (1.4f * y) + 20), cementBrickImage, tint, cementBrickSound, 0);
                            if (i == 22)
                                bricks[x, y] = new PowerBrick(new Vector2(brickImage.Width * (1.4f * x) + 295, brickImage.Height * (1.4f * y) + 20), powerBrickImage, tint, powerBrickSound, 0);
                        }
                    }
                    break;
                #endregion
            }
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            switch (currentGameState)
            {
                case GameState.Start:
                    spriteBatch.DrawString(UiFont, "Total Score: " + m_score + "\nBricksDestroyed: " + brickNumber, new Vector2(440, 440), Color.White);
                    break;
                case GameState.Menu:
                    mainmenu.Draw(spriteBatch);
                    break;
#region LEVEL 1 DRAW
                case GameState.Level1:
                    background.Draw(spriteBatch);
                    noise.Draw(spriteBatch);
                    foreach (Brick brick in bricks)
                         brick.Draw(spriteBatch, arrowmax, arrowmin);
                    playerBar.Draw(spriteBatch, arrowmin, arrowmax);
                    noise.Draw(spriteBatch);
                    ball.Draw(spriteBatch, "first");
                    
                    if(!started)
                        spriteBatch.DrawString(UiFont, "Press Space to Start!", new Vector2(512,400), Color.White);
                    if (powerupActive)
                    {
                        secondBall.Draw(spriteBatch, "second");
                    }
                    claustrophobiaBar.Draw(spriteBatch);
                    spriteBatch.DrawString(UiFont, "Total Score: " + m_score + "\nLevel: " + currentGameState  + "\nRemaining Bricks " + remainingBricks, new Vector2(20, 20), Color.Black);
                    if (claustrophobiaPoints < 400)
                        spriteBatch.DrawString(UiFont, "Claustrophobia: " + (int)claustrophobiaPoints, new Vector2(20, 225), Color.Black);
                    else
                        spriteBatch.DrawString(UiFont, "Claustrophobia: " + (int)claustrophobiaPoints + "!!!", new Vector2(20, 225), Color.Red);
                    spriteBatch.DrawString(UiFont, "Noise Level", new Vector2(90, 670), Color.Black);
                    if (powerupActive)
                        spriteBatch.DrawString(UiFont, "MULTIBALL TIME" + "\n" + (int)powerUpTimer, new Vector2(20, 95), Color.Red); 
                    break;
#endregion
#region LEVEL 2 DRAW
                case GameState.Level2:
                    background.Draw(spriteBatch);
                    noise.Draw(spriteBatch);
                    foreach (Brick brick in bricks)
                         brick.Draw(spriteBatch, arrowmax, arrowmin);
                    playerBar.Draw(spriteBatch, arrowmin, arrowmax);
                    noise.Draw(spriteBatch);
                    ball.Draw(spriteBatch, "first");
                    if (!started)
                        spriteBatch.DrawString(UiFont, "Press Space to Start!", new Vector2(512, 400), Color.White);
                    if (powerupActive)
                    {
                        secondBall.Draw(spriteBatch, "second");
                    }
                    claustrophobiaBar.Draw(spriteBatch);
                    spriteBatch.DrawString(UiFont, "Total Score: " + m_score + "\nLevel: " + currentGameState  + "\nRemaining Bricks " + remainingBricks, new Vector2(20, 20), Color.Black);
                    if (claustrophobiaPoints < 400)
                        spriteBatch.DrawString(UiFont, "Claustrophobia: " + (int)claustrophobiaPoints, new Vector2(20, 225), Color.Black);
                    else
                        spriteBatch.DrawString(UiFont, "Claustrophobia: " + (int)claustrophobiaPoints + "!!!", new Vector2(20, 225), Color.Red);
                    spriteBatch.DrawString(UiFont, "Noise Level", new Vector2(90, 670), Color.Black);
                    if (powerupActive)
                        spriteBatch.DrawString(UiFont, "MULTIBALL TIME" + "\n" + (int)powerUpTimer, new Vector2(20, 95), Color.Red); 
                    break;
#endregion
#region LEVEL 3 DRAW
                case GameState.Level3:
                   background.Draw(spriteBatch);
                    noise.Draw(spriteBatch);
                    foreach (Brick brick in bricks)
                         brick.Draw(spriteBatch, arrowmax, arrowmin);
                    playerBar.Draw(spriteBatch, arrowmin, arrowmax);
                    noise.Draw(spriteBatch);
                    ball.Draw(spriteBatch, "first");
                    if (!started)
                        spriteBatch.DrawString(UiFont, "Press Space to Start!", new Vector2(512, 400), Color.White);
                    if (powerupActive)
                    {
                        secondBall.Draw(spriteBatch, "second");
                    }
                    claustrophobiaBar.Draw(spriteBatch);
                    spriteBatch.DrawString(UiFont, "Total Score: " + m_score + "\nLevel: " + currentGameState  + "\nRemaining Bricks " + remainingBricks, new Vector2(20, 20), Color.Black);
                    if (claustrophobiaPoints < 400)
                        spriteBatch.DrawString(UiFont, "Claustrophobia: " + (int)claustrophobiaPoints, new Vector2(20, 225), Color.Black);
                    else
                        spriteBatch.DrawString(UiFont, "Claustrophobia: " + (int)claustrophobiaPoints + "!!!", new Vector2(20, 225), Color.Red);
                    spriteBatch.DrawString(UiFont, "Noise Level", new Vector2(90, 670), Color.Black);
                    if (powerupActive)
                        spriteBatch.DrawString(UiFont, "MULTIBALL TIME" + "\n" + (int)powerUpTimer, new Vector2(20, 95), Color.Red); 
                    break;
#endregion
                case GameState.Loss:
                    endScreen.Draw(spriteBatch);
                    spriteBatch.DrawString(UiFont, "Total Score: " + m_score + "\nBricksDestroyed: " + brickNumber, new Vector2(440, 440), Color.White);
                    spriteBatch.DrawString(UiFont, "Press Escape to Return", new Vector2(750, 730), Color.White);
                    break;
                case GameState.Win:
                    winScreen.Draw(spriteBatch);
                    spriteBatch.DrawString(UiFont, "Total Score: " + m_score + "\nBricksDestroyed: " + brickNumber, new Vector2(440, 440), Color.White);
                    spriteBatch.DrawString(UiFont, "Press Escape to Return", new Vector2(750, 730), Color.White);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

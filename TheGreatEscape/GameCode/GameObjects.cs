using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PlatformGame;
using TheGreatEscape.GameCode;


namespace TheGreatEscape
{
    internal class Background : StaticGraphic
    {
        public Background(Vector2 Position, Texture2D txrImage) : base(Position, txrImage)
        {
        }
    }

    internal class Menu : StaticGraphic
    {
        private Texture2D menu, tutorial;
        private bool m_state;

        public Menu(Vector2 Position, Texture2D txrImage, Texture2D tutorialImage)
            : base(Position, txrImage)
        {
            tutorial = tutorialImage;
            menu = txrImage;
        }
        public void update(KeyboardState keyboardState, KeyboardState oldKeyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.Escape) && oldKeyboardState.IsKeyUp(Keys.Escape) && m_state)
            {
                m_state = false;
            }
            if (keyboardState.IsKeyDown(Keys.H) && oldKeyboardState.IsKeyUp(Keys.H) && !m_state)
            {
                m_state = true;
            }
        }
        public override void Draw(SpriteBatch sBatch)
        {
            if (m_state)
                sBatch.Draw(tutorial, m_position, Color.White);
            else
                sBatch.Draw(m_txr, m_position, Color.White);
        }
    }
    internal class End : StaticGraphic
    {
        public End(Vector2 Position, Texture2D txrImage)
            : base(Position, txrImage)
        {
        }
    }
    internal class Win : StaticGraphic
    {
        public Win(Vector2 Position, Texture2D txrImage)
            : base(Position, txrImage)
        {
        }
    }
    internal class Claustrophobia : StaticGraphic
    {
        
        private float m_claustrophobia;

        public Claustrophobia(Vector2 Position, Texture2D txrImage, float claustrophobia)
            : base(Position, txrImage)
        {
            m_claustrophobia = claustrophobia;
        }
        public void Update(float claustrophobia)
        {
            m_claustrophobia = claustrophobia;
            m_claustrophobia = (int)MathHelper.Clamp(m_claustrophobia, 0,500);
        }
        public override void Draw(SpriteBatch sBatch)
        {
            //Draw the negative space for the claustrophobia bar
            sBatch.Draw(m_txr, new Rectangle(25, 750, 25, 500), new Rectangle(0, 0, 0, 1), Color.Gray, 0, new Vector2(0, 1), SpriteEffects.FlipVertically, 0);
            //Draw the current health level based on the current claustrophobia
            sBatch.Draw(m_txr, new Rectangle(25, 750, 25, (int)m_claustrophobia), new Rectangle(0, 0,0, 1), Color.White, 0, new Vector2(0,1),  SpriteEffects.FlipVertically, 0);
        }
    }
    internal class Noise : StaticGraphic
    {
        private int m_noise = 3;

        public Noise(Vector2 Position, Texture2D txrImage)
            : base(Position, txrImage)
        {
        }
        public void Update(int noise)
        {
            m_noise = noise;
        }
        public override void Draw(SpriteBatch sBatch)
        {
            //Draw the negative space for the noise bar
            sBatch.Draw(m_txr, m_position, Color.White);
            //Draw the negative space for the noise bar
            sBatch.Draw(m_txr, m_position + new Vector2(60, 0), Color.White);
            //Draw the negative space for the noise bar
            sBatch.Draw(m_txr, m_position + new Vector2(120, 0), Color.White);
            //Draw the current noise level based on the current Health
            if (m_noise <= 1)
            {
                sBatch.Draw(m_txr, m_position, Color.Gray);
            }
            //Draw the current noise level based on the current Health
            if (m_noise == 2)
            {
                sBatch.Draw(m_txr, m_position, Color.Gray);
                sBatch.Draw(m_txr, m_position + new Vector2(60, 0), Color.Gray);
            }
            //Draw the current noise level based on the third Health
            if (m_noise >= 3)
            {
                sBatch.Draw(m_txr, m_position + new Vector2(120, 0), Color.Gray);
                sBatch.Draw(m_txr, m_position + new Vector2(60, 0), Color.Gray);
                sBatch.Draw(m_txr, m_position, Color.Gray);
            }
        }
    }
    /// <summary>
    /// This class controls everything about the player bar. It inherits from the Square Class.
    /// </summary>
    internal class Bar : Square
    {
        private float m_inertia = 0.90f;
        private Rectangle m_Screenbounds;
        private bool atLeft, atRight;
        /// <summary>
        /// Constructor of the Bar, assigns the gamne screen
        /// </summary>
        /// <param name="position"></param> The position of the bar
        /// <param name="txr"></param> The chosen image of the bar
        /// <param name="screenbounds"></param> The game screen size
        public Bar(Vector2 position, Texture2D txr, Rectangle screenbounds) : base(position, txr)
        {
            m_Screenbounds = screenbounds;
        }
        /// <summary>
        /// Changes the bars position by the velocity which changes when the corrent input is given. 
        /// The velocity is constantly multiplied by inertia making it continuously smaller.
        /// Finally the LockBar funtion is called which keeps the bar in it's proper place.
        /// </summary>
        /// <param name="keyboard"></param> The state of the Keyboard
        /// <param name="currPad"></param> The state of the Gamepad of player 1
        /// <param name="oldPad"></param> The previous state of the Gamepad of player 1
        /// <param name="screenbounds"></param> The borders of the game screen
        public void Update(KeyboardState keyboard, GamePadState currPad, GamePadState oldPad, Rectangle screenbounds)
        {
            m_Rect = GetBounds();
            m_Box = GetBoundingBox();
            m_velocity *= m_inertia;
            if (keyboard.IsKeyDown(Keys.Left) && !atLeft||
                keyboard.IsKeyDown(Keys.A) && !atLeft || currPad.IsButtonDown(Buttons.LeftThumbstickLeft) && !atLeft ||
                 currPad.IsButtonDown(Buttons.DPadLeft) && !atLeft)
            {
                m_velocity.X -= 0.9f;
            }
            if (keyboard.IsKeyDown(Keys.Right) && !atRight || (keyboard.IsKeyDown(Keys.D))&& !atRight ||
                currPad.IsButtonDown(Buttons.LeftThumbstickRight) && !atRight || currPad.IsButtonDown(Buttons.DPadRight)&& !atRight)
            {
                m_velocity.X += 0.9f;
            }
            m_position.X += m_velocity.X;
            LockBar();
        }
        /// <summary>
        /// This Function gets the bars current velocity.
        /// </summary>
        public Vector2 GetBarSpeed()
        {
            return m_velocity;
        }
        /// <summary>
        /// This Function Ensures that the Bar stays within it's borders.
        /// </summary>
        private void LockBar()
        {
            if (m_position.X < 282)
            {
                m_position.X = 282;
                atLeft = true;
            }
            else
            {
                atLeft = false;
            }
            if (m_position.X + m_txr.Width > m_Screenbounds.Width - 55)
            {
                m_position.X = m_Screenbounds.Width - m_txr.Width - 55;
                atRight = true;
            }
            else
            {
                atRight = false;
            }
    }
        /// <summary>
        /// Sets the Bar in it's Original position
        /// </summary>
        public void SetInStartPosition()
        {
            m_position.X = (m_Screenbounds.Width - m_txr.Width)/2f + 150;
            m_position.Y = m_Screenbounds.Height - m_txr.Height - 100;
        }
    }
    /// <summary>
    /// This class controls the ball. It inherites from the MotionGraphic Class
    /// </summary>
    internal class Ball : Circle
    {
        private Rectangle m_Screenbounds;
        private Rectangle m_bounds;
        private float ballSpeed = 1;
        private bool m_collided;
        private SoundEffect m_hit;
        public Ball(Vector2 position, Texture2D texture, Rectangle screenBounds, SoundEffect hit) : base(position, texture)
        {
            m_Screenbounds = screenBounds;
            m_hit = hit;
        }
        public void SetPosition(Vector2 position)
        {
            m_position = position;
        }

        public Vector2 GetPosition()
        {
            return m_position;
        }
        public void Update(float speed)
        {
            m_velocity.Y += 0.01f;
            m_sphere = UtilityFunctions.CreateSphere(m_position, m_txr);
            m_collided = false;
            m_position += m_velocity*ballSpeed;
            CheckWallCollision();
            ballSpeed = speed;
            MathHelper.Clamp(m_velocity.Y, ballSpeed, 2f);
            m_velocity.X = MathHelper.Clamp(m_velocity.X, -1f, 1f);
            m_velocity.Y = MathHelper.Clamp(m_velocity.Y, -2.5f, 2f);
        }
        /// <summary>
        /// Establish the bounds within the ball can be. If the ball hits these bounds it will bounce off.
        /// </summary>
        private void CheckWallCollision()
        {
            if (m_position.X < 280)
            {
                m_position.X = 280;
                
                m_velocity.X *= -1;
                if (m_velocity.X > 0.2f)
                    m_velocity.X -= 0.2f;
                
                m_hit.Play();
            }
            if (m_position.X + m_txr.Width > m_Screenbounds.Width - 55)
            {
                m_position.X = m_Screenbounds.Width - m_txr.Width - 55;
                
                m_velocity.X *= -1;
                if (m_velocity.X > 0.2f)
                    m_velocity.X += 0.2f;
                m_hit.Play();
            }
            if (m_position.Y < 0)
            {
                m_position.Y = 0;
                m_velocity.Y *= -1;
                m_hit.Play();
            }
             if (m_position.Y > m_Screenbounds.Height)
             {
                 
                 m_velocity.Y = 5;
             }
        
        }
        /// <summary>
        /// Give the ball a random velocity, makes sure that it is going upwards.
        /// </summary>
        public void ReleaseBall()
        {
            Random rand = new Random();
            m_velocity = new Vector2(rand.Next(-10, 10), -rand.Next(30, 60));
            m_velocity.Normalize();
            m_velocity.Y -= 5;
        }
        /// <summary>
        /// Return the ball on the middle of the paddle and give it a random velocity
        /// </summary>
        /// <param name="barLocation"></param>
        public void SetInStartPosition(Rectangle barLocation)
        {
            m_position.Y = barLocation.Y - m_txr.Height;
            m_position.X = barLocation.X + (barLocation.Width - m_txr.Width)/2;
        }
        /// <summary>
        /// If the ball passes the bottom border return true
        /// </summary>
        /// <returns></returns>
        public bool OffBottom()
        {
            if (m_position.Y > m_Screenbounds.Height)
                return true;
            return false;
        }

        /// <summary>
        /// Detect which side of the bar the ball has hit and then change the balls velocity accordingly
        /// </summary>
        /// <param name="barBox"></param>
        public void BarCollision(BoundingBox barBox, float speedHit, Bar bar)
        {
            if (barBox.Intersects(m_sphere))
            {
                m_velocity.Y = -m_velocity.Y - speedHit;
                m_position.Y = barBox.Min.Y - m_txr.Height;
                m_hit.Play();
                if (m_sphere.Center.X + (m_sphere.Center.X/2) > barBox.Min.X + barBox.Max.X/2)
                    m_velocity.X = Math.Abs(m_velocity.X) + bar.GetBarSpeed().X/10;
                else
                    m_velocity.X = Math.Abs(m_velocity.X) * -1  +bar.GetBarSpeed().X / 10;
            }
        }
        /// <summary>
        /// Decides how to deflect the ball
        /// </summary>
        /// <param name="brick"></param>
        public void Deflection(Brick brick)
        {
            if (!m_collided)
            {
                m_velocity.Y *= -1;
                m_collided = true;
                m_hit.Play();
            }
        }
        public void Draw(SpriteBatch sBatch, string type)
        {
            if(type == "first")
                base.Draw(sBatch);
            else
            {
                sBatch.Draw(m_txr,m_position, Color.Yellow);
            }
        }
        public void SetX(float X)
        {
            m_velocity.X = X;
        }

        public void SetY(float Y)
        {
            m_velocity.Y = Y;
        }
    }
    /// <summary>
    /// The Brick Parent and default class
    /// </summary>
    internal class Brick : Square
    {
        protected int m_score, m_bricksKilled;
        protected float m_claustrophobia;
        protected Color m_tint;
        protected SoundEffect m_hit;
        protected bool m_alive;
        protected float m_powerupTimer;
        // Different brick IDS
        protected const int ID_NOT_ASSIGNED = 0;
        protected const int NORMAL = 1;
        protected const int GLASS = 2;
        protected const int CEMENT = 3;
        /// <summary>
        /// Create a new brick with the given ID
        /// </summary>
        /// <param name="id"></param>
        public Brick(Vector2 position, Texture2D txrImage, Color tint, SoundEffect hit, int id) : base(position, txrImage)
        {
            m_hit = hit;
            m_tint = tint;
            m_alive = true;
        }
        /// <summary>
        /// Gives the position of the brick
        /// </summary>
        /// <returns></returns>
        public Vector2 ReturnPosition()
        {
            return m_position;
        }
        public void Update(GameTime gt, float claustrophobia, int score, int brickskilled)
        {
            m_claustrophobia = claustrophobia;
            m_score = score;
            m_bricksKilled = brickskilled;
            base.Update(gt);
        }
        public int BricksKilled()
        {
            return m_bricksKilled;
        }
        public int AssignScore()
        {
            return m_score;
        }
        public float AssignPowerUp()
        {
            return m_powerupTimer;
        }
        public float AssignPoints()
        {
            return m_claustrophobia;
        }
        /// <summary>
        /// Check if the bakll has collided with this brick
        /// </summary>
        /// <param name="ball"></param>
        /// <returns></returns>
        public bool Collided(Ball ball)
        {
            if (m_alive && ball.GetSphere().Intersects(m_Box))
                return true;
            return false;
        }
        /// <summary>
        /// Apply the logic for a collision
        /// </summary>
        /// <param name="ball"></param>
        public virtual void OnCollide(Ball ball)
        {
            m_alive = false;
            ball.Deflection(this);
            m_hit.Play();
            m_claustrophobia -= 10;
            m_score += 10;
            m_bricksKilled += 1;
        }
        /// <summary>
        /// Check if a brink is alive and return a bool
        /// </summary>
        /// <returns></returns>
        public virtual bool CheckCondition()
        {
            if (!m_alive)
            {
                return true;

            }
            return false;
        }
        public override void Draw(SpriteBatch sBatch, Texture2D box_max, Texture2D box_min)
        {
            if (m_alive)
                sBatch.Draw(m_txr, m_position, m_tint);
#if DEBUG
            sBatch.Draw(box_min, UtilityFunctions.V3ToV2(m_Box.Min) - new Vector2(1, 1), Color.White);
            sBatch.Draw(box_max,
                (UtilityFunctions.V3ToV2(m_Box.Max) - new Vector2(box_max.Width, box_max.Height)) + Vector2.One,
                Color.White);
#endif
        }
    }
    /// <summary>
    /// The original brick, parent of all bricks
    /// </summary>
    internal class BrickNormal : Brick
    {
        public BrickNormal(Vector2 position, Texture2D txrImage, Color tint, SoundEffect hit, int id) : base(position, txrImage, tint, hit, NORMAL)
        {
        }
    }
    /// <summary>
    /// When destroyed, the ball does not “bounce” and change direction.
    /// </summary>
    internal class GlassBrick : Brick
    {
        public GlassBrick(Vector2 position, Texture2D txrImage, Color tint, SoundEffect hit, int id)
            : base(position, txrImage, tint, hit, GLASS)
        {
        }
        public override void OnCollide(Ball ball)
        {
            m_alive = false;
            m_hit.Play();
            m_claustrophobia -= 10;
            m_score += 20;
            m_bricksKilled += 1;
        }
    }
        /// <summary>
        /// When destroyed, the ball loses a life. Only when all three of its lives are gone can it be destroyed.
        /// </summary>
        internal class MetalBrick : Brick
        {
            private int m_lives = 3;
            public MetalBrick(Vector2 position, Texture2D txrImage, Color tint, SoundEffect hit, int id)
                : base(position, txrImage, tint, hit, GLASS)
            {
            }

            public override void OnCollide(Ball ball)
            {
                if (m_lives > 0)
                {
                    ball.Deflection(this);
                    m_lives -= 1;
                    m_hit.Play();
                }
                if (m_lives == 0)
                {
                    ball.Deflection(this);
                    m_alive = false;
                    m_hit.Play();
                    m_claustrophobia -= 15;
                    m_score += 30;
                    m_bricksKilled += 1;
                }
            }
        }
        /// <summary>
        /// When destroyed, the ball loses a life. Only when all three of its lives are gone can it be destroyed.
        /// </summary>
        internal class PowerBrick : Brick
        {
            public PowerBrick(Vector2 position, Texture2D txrImage, Color tint, SoundEffect hit, int id)
                : base(position, txrImage, tint, hit, GLASS)
            {
            }
            public override void OnCollide(Ball ball)
            {
                ball.Deflection(this);
                m_alive = false;
                m_hit.Play();
                m_claustrophobia -= 15;
                m_score += 30;
                m_bricksKilled += 1;
               m_powerupTimer = 120;
            }
        }
}

using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FarseerPhysics.DebugViews;
using FarseerPhysics;
using System.Collections.Generic;
using System;
using FarseerPhysics.Common;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using Dhpoware;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace BitSmith
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        public static GraphicsDeviceManager graphics;
        private static SpriteBatch batch;
        private MouseState mouseStateCurrent, mouseStatePrevious;
        private KeyboardState oldKeyState;
        private GamePadState oldPadState;
        private SpriteFont font;
        private SpriteFont minecraftia;
        private SpriteFont bitWonder;
        public static Vector2 mouseTruePos;
        private BloomComponent bloom;
        public static RenderTarget2D scene;
        RenderTarget2D sceneReg;
        RenderTarget2D sceneRegB;
        RenderTarget2D sceneRegPre;
        Water water;
        Texture2D particleImage;

        int xPos;
        int yPos;

        byte blendAmount = 40;

        PostProcessingManager ppm;
        CrepuscularRays rays;
        Vector2 rayStartingPos = new Vector2(0, 1);

        public static World world;
        public static FluidSimulation _fluidSimulation;

        private Body circleBody;
        private Body groundBody;
        private Body characterBody;
        private Body armBody;
        private Vector2 prevPos;
        private bool notMoving = false;
        private bool _collision;
        private bool _touchingFloor;

        private const int JUMP_COOLDOWN = 20;
        private int jumpCooldown = JUMP_COOLDOWN;

        private static Random random;

        private Texture2D circleSprite;
        private Texture2D blissSprite;
        private Texture2D characterSprite;
        private Texture2D vignette;
        private float sunRad;
        private float shadowAmount;

        private Song menuMusic;

        private SoundEffect menuBlip1;
        private SoundEffect menuBlip2;

        private const int xD = 256*4;
        private const int yD = 256;
        public static Block[,] terrain = new Block[xD, yD];
        private static byte totalImages = 255;
        private static Texture2D walk;
        private static Texture2D stand;
        private static Texture2D arm1;
        private static Texture2D arm2;
        private static Texture2D sword;
        private static Texture2D pick;
        private static Texture2D back;
        private static Texture2D[] textures = new Texture2D[totalImages];
        private const int grassTextures = 8;
        private static Texture2D[] grass = new Texture2D[grassTextures];
        private static List<Vector2> deleteBlocks = new List<Vector2>();

        private List<Body> objects = new List<Body>();

        private const float rDF = 0;
        private const float gDF = 127;
        private const float bDF = 255;

        private const float rDI = 0;
        private const float gDI = 0;
        private const float bDI = 40;

        private static float rD = 0;
        private static float gD = 0;
        private static float bD = 40;

        Color dayTime = new Color(rD, gD, bD);

        private const float rFF = 255;
        private const float gFF = 255;
        private const float bFF = 255;

        private const float rFI = 20;
        private const float gFI = 20;
        private const float bFI = 20;

        private static float rF = 20;
        private static float gF = 20;
        private static float bF = 20;

        Color frontTime = new Color(rF, gF, bF);

        // Simple camera controls
        private Matrix _view;
        public static Camera2d cam;
        private Vector2 _screenCenter;

        public const int WINDOW_WIDTH = 1280;
        public const int WINDOW_HEIGHT = 720;

        // physics simulator debug view
        DebugViewXNA _debugView;

        private List<Cloud> clouds = new List<Cloud>();

        public const float scale = 16f;

        private static GaussianBlur gaussianBlur;

        private const int BLUR_RADIUS = 1;
        private const float BLUR_AMOUNT = 0.0001f;

        private static RenderTarget2D renderTarget1;
        private static RenderTarget2D renderTarget2;

        private static Texture2D result;

        private int renderTargetWidth;
        private int renderTargetHeight;

        private static Boolean blur = false;

        private bool change = false;
        private int direction;
        private static int COOLDOWN = 12;
        private static int cooldown = COOLDOWN;
        private static int maxSpeed = 6;

        private static float zoom = 1.0f;
        private int drawAmount = 0;
        RevoluteJoint _joint;
        private Boolean usingArm = true;
        public static Boolean strafeMode = false;
        private Boolean debug = true;
        private Boolean landed = false;
        float degree = 45;

        float weoponDegree = 0;
        Vector2 weoponPos;

        private Effect fxaaEffect;

        private bool useFXAA = true;

        private const float MainImageDepth = 0.9f;
        private const float ThingsInFrontDepth = 0.5f;
        private float N = 0.40f;
        private float subPixelAliasingRemoval = 0.75f;
        private float edgeTheshold = 0.166f;
        private float edgeThesholdMin = 0f;
        private float consoleEdgeSharpness = 8.0f;
        private float consoleEdgeThreshold = 0.125f;
        private float consoleEdgeThresholdMin = 0f;

        bool menuEnabled = true;
        bool fadeOut = false;
        bool fadeOutSwitch = false;
        float fadeOutNum = 0;
        bool resetMenuButtonController = false;
        int menuNum = 1;

        float charmNum = 2;
        Vector2 worldPosition;
        Vector2 worldPosition1;
        Vector2 worldPosition2;
        Rectangle backgroundRectangle;

#if !XBOX360
        const string Text = "Press A or D to rotate the ball\n" +
                            "Press Space to jump\n" +
                            "Press Shift + W/S/A/D to move the camera";
#else
                const string Text = "Use left stick to move\n" +
                                    "Use right stick to move camera\n" +
                                    "Press A to jump\n";
#endif
        #region "consctrucitn"
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            graphics.PreferMultiSampling = false;
            graphics.PreferredBackBufferFormat = SurfaceFormat.Color;
            graphics.SynchronizeWithVerticalRetrace = true;
            IsFixedTimeStep = false;
            
            //graphics.IsFullScreen = true;
            Random random = new Random();
            Cloud test = new Cloud(new Vector2(-100 + WINDOW_WIDTH, 100));
            clouds.Add(test);

            Cloud test2 = new Cloud(new Vector2(-100 + (WINDOW_WIDTH*3)/4, 50));
            clouds.Add(test2);

            Cloud test3 = new Cloud(new Vector2(50, 180));
            clouds.Add(test3);

            /*Cloud t2est2 = new Cloud(new Vector2(600 + random.Next(-700, 700), 0 + random.Next(-10, 200)));
            clouds.Add(t2est2);

            Cloud t3est3 = new Cloud(new Vector2(600 + random.Next(-700, 700), 0 + random.Next(-10, 200)));
            clouds.Add(t3est3);*/

            Content.RootDirectory = "Content";

            world = new World(new Vector2(0, 40));

            Components.Add(new FrameRateCounter(this));
            bloom = new BloomComponent(this);
            Components.Add(bloom);
            ppm = new PostProcessingManager(this);

            rays = new CrepuscularRays(this, Vector2.One * .5f, "Textures/flare", 1, .99f, .98f, .5f, .25f);

            ppm.AddEffect(rays);
            
            this.IsMouseVisible = true;
        }
        #endregion
        #region "loadcontent"
        protected override void LoadContent()
        {
            // Initialize camera controls
            #region "etc"
            _view = Matrix.Identity;

            _screenCenter = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2f,
                                                graphics.GraphicsDevice.Viewport.Height / 2f);

            backgroundRectangle = new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);

            //_cameraPosition = new Vector2(0, -200);
            cam = new Camera2d();
            cam._pos = new Vector2(0, -200);
            _view = Matrix.CreateTranslation(new Vector3(cam._pos - _screenCenter, 0f)) *
                        Matrix.CreateTranslation(new Vector3(_screenCenter, 0f));
            batch = new SpriteBatch(graphics.GraphicsDevice);

            menuMusic = Content.Load<Song>("Audio/MenuMusic");  // Put the name of your song in instead of "song_title"
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.6f;
            MediaPlayer.Play(menuMusic);

            menuBlip1 = Content.Load<SoundEffect>("Audio/Blip1");
            menuBlip2 = Content.Load<SoundEffect>("Audio/Blip2");

            font = Content.Load<SpriteFont>("font");

            minecraftia = Content.Load<SpriteFont>("Fonts/Minecraftia");
            bitWonder = Content.Load<SpriteFont>("Fonts/8BitWonder");

            fxaaEffect = Content.Load<Effect>("FXAA");

            // Load sprites
            circleSprite = Content.Load<Texture2D>("circleSprite"); //  96px x 96px => 1.5m x 1.5m
            //groundSprite = Content.Load<Texture2D>("groundSprite"); // 512px x 64px =>   8m x 1m
            blissSprite = Content.Load<Texture2D>("bliss");
            vignette = Content.Load<Texture2D>("textures/vignette");
            particleImage = Content.Load<Texture2D>("textures/metaparticle");

            walk = Content.Load<Texture2D>("textures/walk"); //  96px x 96px => 1.5m x 1.5m
            stand = Content.Load<Texture2D>("textures/stand"); // 512px x 64px =>   8m x 1m
            arm1 = Content.Load<Texture2D>("textures/arm"); // 512px x 64px =>   8m x 1m
            sword = Content.Load<Texture2D>("textures/sword");
            pick = Content.Load<Texture2D>("textures/pick");
            back = Content.Load<Texture2D>("textures/back");

            /* Circle */
            circleSprite = Content.Load<Texture2D>("circleSprite"); //  96px x 96px => 1.5m x 1.5m
            //groundSprite = Content.Load<Texture2D>("groundSprite"); // 512px x 64px =>   8m x 1m
            characterSprite = Content.Load<Texture2D>("textures/character"); // 512px x 64px =>   8m x 1m

            textures[1] = Content.Load<Texture2D>("textures/blocks/stone");
            textures[2] = Content.Load<Texture2D>("textures/blocks/dirt");
            textures[3] = Content.Load<Texture2D>("textures/blocks/iron");
            textures[4] = Content.Load<Texture2D>("textures/blocks/gold");
            textures[10] = Content.Load<Texture2D>("textures/blocks/cloud");
            textures[11] = Content.Load<Texture2D>("textures/blocks/grass");
            textures[12] = Content.Load<Texture2D>("textures/blocks/wood");
            textures[13] = Content.Load<Texture2D>("textures/blocks/brush");

            grass[1] = Content.Load<Texture2D>("textures/grass/grassleft");
            grass[2] = Content.Load<Texture2D>("textures/grass/grasstop");
            grass[3] = Content.Load<Texture2D>("textures/grass/grassright");
            grass[4] = Content.Load<Texture2D>("textures/grass/grasstopleft");
            grass[5] = Content.Load<Texture2D>("textures/grass/grasstopright");
            grass[6] = Content.Load<Texture2D>("textures/grass/grassleftright");
            grass[7] = Content.Load<Texture2D>("textures/grass/grasscovered");

            _fluidSimulation = new FluidSimulation(world, batch, font);
            water = new Water(GraphicsDevice, particleImage);
            #endregion
            #region "initTerrain"
            for (int x = 0; x < xD; x++)
            {
                for (int y = 0; y < yD; y++)
                {
                    terrain[x, y] = new Block();
                    terrain[x, y].setBlockType(Block.NO_BLOCK);
                }
            }
            int Seed = (int)DateTime.Now.Ticks;
            Console.Out.WriteLine(Seed);
            random = new Random(Seed);
            /*for (int xi = 1; xi < xD; xi++)
            {
                for (int yi = 1; yi < yD - 121; yi++)
                {
                    terrain[xi, yi + 121].setBlockType(2);
                }
            }*/
            Tree temp = new Tree(157, 121);
            terrain[159, 120].setBlockType(2);
            for (int xi = 163; xi < xD / 4; xi++)
            {
                terrain[xi, 117].setBlockType(2);
            }
            for (int xi = 162; xi < xD / 4; xi++)
            {
                terrain[xi, 118].setBlockType(2);
            }
            for (int xi = 161; xi < xD / 4; xi++)
            {
                terrain[xi, 119].setBlockType(2);
            }
            for (int xi = 161; xi < xD / 4; xi++)
            {
                terrain[xi, 120].setBlockType(2);
            }
            for (int xi = 150; xi < xD/4; xi++)
            {
                terrain[xi, 121].setBlockType(2);
            }
            for (int xi = 151; xi < xD / 4; xi++)
            {
                terrain[xi, 122].setBlockType(2);
            }
            for (int xi = 152; xi < xD / 4; xi++)
            {
                terrain[xi, 123].setBlockType(2);
            }
            for (int xi = 154; xi < xD / 4; xi++)
            {
                terrain[xi, 124].setBlockType(2);
            }
            for (int xi = 156; xi < xD / 4; xi++)
            {
                terrain[xi, 125].setBlockType(2);
            }
            for (int xi = 159; xi < xD / 4; xi++)
            {
                terrain[xi, 126].setBlockType(2);
            }
            for (int xi = 161; xi < xD / 4; xi++)
            {
                terrain[xi, 127].setBlockType(2);
            }
            for (int xi = 162; xi < xD / 4; xi++)
            {
                terrain[xi, 128].setBlockType(2);
            }
            //rock
            for (int xi = 163; xi < xD / 4; xi++)
            {
                terrain[xi, 119].setBlockType(1);
            }
            for (int xi = 162; xi < xD / 4; xi++)
            {
                terrain[xi, 120].setBlockType(1);
            }
            for (int xi = 162; xi < xD / 4; xi++)
            {
                terrain[xi, 121].setBlockType(1);
            }
            for (int xi = 153; xi < xD / 4; xi++)
            {
                terrain[xi, 122].setBlockType(1);
            }
            for (int xi = 155; xi < xD / 4; xi++)
            {
                terrain[xi, 123].setBlockType(1);
            }
            for (int xi = 157; xi < xD / 4; xi++)
            {
                terrain[xi, 124].setBlockType(1);
            }
            for (int xi = 160; xi < xD / 4; xi++)
            {
                terrain[xi, 125].setBlockType(1);
            }
            for (int xi = 162; xi < xD / 4; xi++)
            {
                terrain[xi, 126].setBlockType(1);
            }
            //dem checks
            for (int xi = 2; xi < xD - 1; xi++)
            {
                for (int yi = 1; yi < yD - 1; yi++)
                {
                    checkVariation(xi, yi);
                }
            }
            for (int xi = 4; xi < xD - 3; xi++)
            {
                for (int yi = 4; yi < yD - 3; yi++)
                {
                    if (terrain[xi, yi].getBlockType() != Block.DIRT_BLOCK)
                    {
                        terrain[xi, yi].setVariation(0);
                    }
                    //checkGrass(xi, yi);
                }
            }
            terrain[157, 121].setVariation(2);
            terrain[151, 120].setBlockType(Block.GRASS_BLOCK);
            terrain[153, 120].setBlockType(Block.GRASS_BLOCK);
            terrain[154, 120].setBlockType(Block.GRASS_BLOCK);
            terrain[155, 120].setBlockType(Block.GRASS_BLOCK);
            terrain[160, 120].setBlockType(Block.GRASS_BLOCK);
            terrain[162, 117].setBlockType(Block.GRASS_BLOCK);
            #endregion
            //generateTerrain();
            #region "physicssetup"
            // create and configure the debug view
            _debugView = new DebugViewXNA(world);
            _debugView.AppendFlags(DebugViewFlags.DebugPanel);
            _debugView.DefaultShapeColor = Color.White;
            _debugView.SleepingShapeColor = Color.LightGray;
            _debugView.LoadContent(GraphicsDevice, Content);

            Vector2 characterPosition = new Vector2(152.9f, 120);

            characterBody = CreateCapsule(world, 14f / scale, 7f / scale, 1f, null);

            characterBody.Position = characterPosition;
            characterBody.Mass = 10000;
            characterBody.SleepingAllowed = false;
            characterBody.IsStatic = false;
            characterBody.Restitution = 0.0f;
            characterBody.Friction = 0.0f;
            characterBody.FixedRotation = true;
            characterBody.OnCollision += CharacterOnCollision;
            characterBody.OnSeparation += CharacterOnSeparation;

            ////// arm stuff
            /*armBody = BodyFactory.CreateRectangle(world, 6f / scale, 29f / scale, 1f, characterPosition+new Vector2(0/scale,-14/scale));
            armBody.Mass = 1;
            armBody.IsStatic = false;
            armBody.IgnoreCollisionWith(characterBody);

            _joint = new RevoluteJoint(armBody, characterBody, armBody.GetLocalPoint(characterBody.Position), new Vector2(4 / scale, -4 / scale));
            world.AddJoint(_joint);

            //_joint.MotorSpeed = 1.0f * Settings.Pi;
            _joint.MaxMotorTorque = 1.0f;
            _joint.MotorEnabled = false;
            _joint.CollideConnected = false;*/

            characterBody.Mass = 1;
            //armBody.Mass = 0;

            /* Circle */
            // Convert screen center from pixels to meters
            Vector2 circlePosition = (_screenCenter / scale) + new Vector2(0, -40.5f);

            // Create the circle fixture
            circleBody = BodyFactory.CreateCircle(world, 96f / (2f * scale), 1f, circlePosition);
            circleBody.BodyType = BodyType.Dynamic;

            // Give it some bounce and friction
            circleBody.Restitution = 0.3f;
            circleBody.Friction = 0.5f;

            /* Ground */
            Vector2 groundPosition = (_screenCenter / scale) + new Vector2(0, -24.25f);

            // Create the ground fixture
            groundBody = BodyFactory.CreateRectangle(world, 512f / scale, 64f / scale, 1f, groundPosition);
            groundBody.IsStatic = true;
            groundBody.Restitution = 0.3f;
            groundBody.Friction = 0.5f;

            // create and configure the debug view
            _debugView = new DebugViewXNA(world);
            _debugView.AppendFlags(DebugViewFlags.DebugPanel);
            _debugView.DefaultShapeColor = Color.White;
            _debugView.SleepingShapeColor = Color.LightGray;
            _debugView.LoadContent(GraphicsDevice, Content);
            #endregion
            #region "blur"
            gaussianBlur = new GaussianBlur(this);
            gaussianBlur.ComputeKernel(BLUR_RADIUS, BLUR_AMOUNT);

            renderTargetWidth = 16;
            renderTargetHeight = 16;

            renderTarget1 = new RenderTarget2D(GraphicsDevice,
                renderTargetWidth, renderTargetHeight, false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.None);

            renderTarget2 = new RenderTarget2D(GraphicsDevice,
                renderTargetWidth, renderTargetHeight, false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.None);

            gaussianBlur.ComputeOffsets(renderTargetWidth, renderTargetHeight);
            #endregion
            #region "rays"
            Services.AddService(batch.GetType(), batch);

            rays.lightSource = rayStartingPos;
            rays.Exposure -= .15f;
            rays.LightSourceSize -= .5f;

            scene = new RenderTarget2D(graphics.GraphicsDevice, graphics.GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.None);
            sceneReg = new RenderTarget2D(graphics.GraphicsDevice, graphics.GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.None);
            sceneRegB = new RenderTarget2D(graphics.GraphicsDevice, graphics.GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.None);
            sceneRegPre = new RenderTarget2D(graphics.GraphicsDevice, graphics.GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.None);

            #endregion
        }
        #endregion
        #region "update"
        protected override void Update(GameTime gameTime)
        {
            degree+=0.01f;
            if (degree >= 360)
                degree = 0;
            //float rad = (degree * (float)Math.PI / 180);
            float rad;

            rad = (degree * (float)Math.PI / 180);

            if (degree < 180)
            {
                sunRad = ((degree - 180) * (float)Math.PI / 180);
                shadowAmount = (float)Math.Sin(rad)/1.5f;
                rays.Density = 0.8f;
                rays.Weight = 0.8f;
                rays.LightSourceSize = 0.25f;
                rays.Exposure = 0.3f;
            }
            else
            {
                sunRad = (degree * (float)Math.PI / 180);
                shadowAmount = 0;
                rays.Density = 0.75f;
                rays.Weight = 1f;
                rays.LightSourceSize = 0.2f;
            }

            float tempDegree = degree;
            //rays.lightSource = new Vector2(rays.lightSource.X + .00005f, rays.lightSource.Y - .0001f);

            Vector2 cyclePos = new Vector2((float)(Math.Cos(rad) * 500 + (GraphicsDevice.Viewport.Width / 2)) / GraphicsDevice.Viewport.Width,
                (float)(Math.Sin(rad) * 700 + (GraphicsDevice.Viewport.Height * 1.1f)) / GraphicsDevice.Viewport.Height);

            rays.lightSource = new Vector2((float)(Math.Cos(sunRad) * 500 + (GraphicsDevice.Viewport.Width / 2)) / GraphicsDevice.Viewport.Width, 
                (float)(Math.Sin(sunRad) * 700 + (GraphicsDevice.Viewport.Height*1.1f)) / GraphicsDevice.Viewport.Height);

            degree = 90;
            rad = (degree * (float)Math.PI / 180);

            Vector2 maxValues = new Vector2((float)(Math.Cos(rad) * 500 + (GraphicsDevice.Viewport.Width / 2)) / GraphicsDevice.Viewport.Width, 
                (float)(Math.Sin(rad) * 700 + (GraphicsDevice.Viewport.Height*1.1f)) / GraphicsDevice.Viewport.Height);

            degree = 270;
            rad = (degree * (float)Math.PI / 180);

            Vector2 minValues = new Vector2((float)(Math.Cos(rad) * 500 + (GraphicsDevice.Viewport.Width / 2)) / GraphicsDevice.Viewport.Width, 
                (float)(Math.Sin(rad) * 700 + (GraphicsDevice.Viewport.Height*1.1f)) / GraphicsDevice.Viewport.Height);

            degree = tempDegree;

            //float timeVal = (cyclePos.Y+Math.Abs(minValues.Y))/(maxValues.Y+Math.Abs(minValues.Y))-0.1f;
            float timeVal = (cyclePos.Y+Math.Abs(minValues.Y))/(maxValues.Y+Math.Abs(minValues.Y))+0.1f;

            if (timeVal <= 0)
                timeVal = 0;
            else if (timeVal >= 1)
                timeVal = 1;

            rD = rDI + (rDF-rDI)*timeVal;
            gD = gDI + (gDF-gDI)*timeVal;
            bD = bDI + (bDF-bDI)*timeVal;

            dayTime = new Color(rD / 255, gD / 255, bD / 255);

            Console.WriteLine("" + dayTime);

            rF = rFI + (rFF-rFI)*timeVal;
            gF = gFI + (gFF-gFI)*timeVal;
            bF = bFI + (bFF-bFI)*timeVal;

            frontTime = new Color(rF / 255, gF / 255, bF / 255);

            //Console.WriteLine("" + frontTime);

            HandleMouse();
            HandleInput();
            for (int cloudNum = 0; cloudNum < clouds.Count; cloudNum++)
            {
                clouds[cloudNum].update();
            }

            //We update the world
            prevPos = characterBody.Position;
            world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
            _fluidSimulation.update();

            //armBody.Position = characterBody.Position;
            //armBody.
            //armBody.IgnoreGravity = true;
            //_joint.MotorSpeed = 0;
            //_joint.MotorTorque = 0;
            /*_joint.LowerLimit = 0;
            _joint.UpperLimit = 0;
            _joint.MaxMotorTorque = 10;
            _joint.MotorSpeed = (MathHelper.ToDegrees(weoponDegree) + (MathHelper.ToDegrees(armBody.Rotation)%360));
            if (Math.Abs(MathHelper.ToDegrees(weoponDegree) - (MathHelper.ToDegrees(armBody.Rotation) % 360)) >= 5)
            {
                //armBody.Rotation = armBody.Rotation - (MathHelper.ToDegrees(weoponDegree) - (MathHelper.ToDegrees(armBody.Rotation) % 360))/1000;
                _joint.MotorSpeed = 0;
                _joint.MotorTorque = 0;
            }*/
            //armBody.Rotation = weoponDegree;

            if (Math.Abs(prevPos.X - characterBody.Position.X) > 0.5f / scale || Math.Abs(prevPos.Y - characterBody.Position.Y) > 0.5f / scale)
                notMoving = true;


            //camera start offset
            if (!menuEnabled)
            {
                zoom = 2.5f;
                cam._pos = (characterBody.Position * scale) - new Vector2(7, 14);
                cam.Zoom = zoom;
            }
            else if (menuEnabled)
            {
                zoom = 2.0f;
                cam._pos = (characterBody.Position * scale) - new Vector2(7, 14) + new Vector2(-WINDOW_WIDTH / 8, -WINDOW_HEIGHT / 22);
                cam.Zoom = zoom;
            }

            if (fadeOut)
            {
                if (!fadeOutSwitch)
                {
                    fadeOutNum += 0.02f;
                    if (fadeOutNum >= 1)
                    {
                        fadeOutNum = 1;
                        fadeOutSwitch = true;
                        menuEnabled = false;
                        characterBody.Position = new Vector2(100, 0);
                        generateTerrain();
                    }
                }
                else
                {
                    fadeOutNum -= 0.02f;
                    if (fadeOutNum <= 0)
                    {
                        fadeOutNum = 0;
                        fadeOutSwitch = false;
                        fadeOut = false;
                    }
                }
            }
            #region "charactercollsion"
            {
                int row = (int)(characterBody.Position.X + (characterBody.LinearVelocity.X / (scale * 3)));
                int col = (int)(characterBody.Position.Y + (characterBody.LinearVelocity.Y / (scale * 3)));
                for (int xP = -1; xP < 2; xP++)
                {
                    for (int yP = -1; yP < 2; yP++)
                    {
                        if (row > 1 && col > 1 && row < xD - 2 && col < yD - 2)
                        {
                            if (terrain[row + xP, col + yP].getBlockType() != Block.NO_BLOCK && terrain[row + xP, col + yP].getBlockType() != Block.GRASS_BLOCK)
                            {
                                if (terrain[row + xP, col + yP].getBlockBlody() == null)
                                {
                                    terrain[row + xP, col + yP].createBlock(new Vector2(row + xP, col + yP));
                                    //terrain[row + xP, col + yP].getBlockBlody().IgnoreCollisionWith(armBody);
                                    deleteBlocks.Add(new Vector2(row + xP, col + yP));
                                }
                                else
                                    terrain[row + xP, col + yP].setDelete(false);
                            }
                        }
                    }
                }
                for (int i = 0; i < deleteBlocks.Count; i++)
                {
                    if (terrain[(int)deleteBlocks[i].X, (int)deleteBlocks[i].Y].getDelete() == true)
                    {
                        terrain[(int)deleteBlocks[i].X, (int)deleteBlocks[i].Y].deleteBlock();
                        deleteBlocks.Remove(deleteBlocks[i]);
                    }
                    else
                    {
                        terrain[(int)deleteBlocks[i].X, (int)deleteBlocks[i].Y].setDelete(true);
                    }
                }

                Vector2 rayStart = new Vector2(characterBody.Position.X - (6 / scale), characterBody.Position.Y + (14 / scale)); // at the bottom of the player sprite
                Vector2 rayEnd = rayStart + new Vector2(0 / scale, 1 / scale); // check 5 pixels down

                _touchingFloor = false;
                world.RayCast((fixture, point, normal, fraction) =>
                {
                    if (fixture != null)
                    {
                        if (!landed)
                        {
                            landed = true;
                            //change = false;
                            cooldown = -1;
                        }
                        _touchingFloor = true;
                        return 1;
                    }
                    return fraction;
                }
                , rayStart, rayEnd);

                rayStart = new Vector2(characterBody.Position.X, characterBody.Position.Y + (14 / scale)); // at the bottom of the player sprite
                rayEnd = rayStart + new Vector2(0, 1 / scale); // check 5 pixels down

                world.RayCast((fixture, point, normal, fraction) =>
                {
                    if (fixture != null)
                    {
                        if (!landed)
                        {
                            landed = true;
                            //change = false;
                            cooldown = -1;
                        }
                        _touchingFloor = true;
                        return 1;
                    }
                    return fraction;
                }
                , rayStart, rayEnd);

                rayStart = new Vector2(characterBody.Position.X + (67 / scale), characterBody.Position.Y + (14 / scale)); // at the bottom of the player sprite
                rayEnd = rayStart + new Vector2(0 / scale, 1 / scale); // check 5 pixels down

                world.RayCast((fixture, point, normal, fraction) =>
                {
                    if (fixture != null)
                    {
                        if (!landed)
                        {
                            landed = true;
                            //change = false;
                            cooldown = -1;
                        }
                        _touchingFloor = true;
                        return 1;
                    }
                    return fraction;
                }
                , rayStart, rayEnd);
            }
            if (!_touchingFloor)
            {
                cooldown = COOLDOWN;
                landed = false;
                change = true;
            }
            if (jumpCooldown < JUMP_COOLDOWN)
            {
                jumpCooldown--;
                if (jumpCooldown <= 0)
                    jumpCooldown = JUMP_COOLDOWN;
            }
            #endregion
            #region "objectcollsion"
            for (int zi = 0; zi < objects.Count; zi++)
            {
                if (objects[zi].Awake)
                {
                    int row = (int)(objects[zi].Position.X + (objects[zi].LinearVelocity.X / (scale * 2)));
                    int col = (int)(objects[zi].Position.Y + (objects[zi].LinearVelocity.Y / (scale * 2)));
                    for (int xP = -1; xP < 2; xP++)
                    {
                        for (int yP = -1; yP < 2; yP++)
                        {
                            if (row > 1 && col > 1 && row < xD - 2 && col < yD - 2)
                            {
                                if (terrain[row + xP, col + yP].getBlockType() != Block.NO_BLOCK && terrain[row + xP, col + yP].getBlockType() != Block.GRASS_BLOCK)
                                {
                                    if (terrain[row + xP, col + yP].getBlockBlody() == null)
                                    {
                                        terrain[row + xP, col + yP].createBlock(new Vector2(row + xP, col + yP));
                                        deleteBlocks.Add(new Vector2(row + xP, col + yP));
                                    }
                                    else
                                        terrain[row + xP, col + yP].setDelete(false);
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            water.Update();
            base.Update(gameTime);
        }
        #endregion
        #region "mouse"
        private void HandleMouse()
        {
            mouseStateCurrent = Mouse.GetState();
            xPos = mouseStateCurrent.X;
            yPos = mouseStateCurrent.Y;
            worldPosition = Vector2.Transform(new Vector2(xPos, yPos), Matrix.Invert(cam.get_transformation(graphics.GraphicsDevice)));
            float mousePosX = worldPosition.X / scale;
            float mousePosY = worldPosition.Y / scale;
            mouseTruePos = new Vector2(mousePosX, mousePosY);
            //charmNum = 2;
            //float mousePosX = ((cam._pos.X + xPos - WINDOW_WIDTH / 2) / scale) - ((cam.Zoom) * charmNum ) + ((1) * charmNum);
            //float mousePosY = ((cam._pos.Y + yPos - WINDOW_HEIGHT / 2) / scale) - ((cam.Zoom) * charmNum) + ((1) * charmNum);
            //float mousePosX = ((((cam._pos.X + -WINDOW_WIDTH / 2) / (cam.Zoom))+xPos) / scale);
            //float mousePosY = ((((cam._pos.Y + -WINDOW_HEIGHT / 2) / (cam.Zoom))+yPos) / scale);
            //float mousePosX = (((((cam._pos.X + -WINDOW_WIDTH / 2)) + xPos) / cam.Zoom) / scale);
            //float mousePosY = (((((cam._pos.Y + -WINDOW_HEIGHT / 2)) + yPos) / cam.Zoom) / scale);
            if (mouseStateCurrent.LeftButton == ButtonState.Pressed && mouseStatePrevious.LeftButton == ButtonState.Released && !menuEnabled && strafeMode)
            {
                //Console.WriteLine((int)((cam._pos.X + xPos + 8) / 16));
                /*if (mousePosX > 0 && mousePosX < xD && mousePosY > 0 && mousePosY < yD)
                {
                    if (terrain[(int)mousePosX, (int)mousePosY].getBlockType() != Block.NO_BLOCK)
                        terrain[(int)mousePosX, (int)mousePosY].setBlockType(Block.NO_BLOCK);
                    else
                    {
                        terrain[(int)mousePosX, (int)mousePosY].setBlockType(Block.DIRT_BLOCK);
                        //checking for variation from mouse click
                        checkVariation((int)mousePosX + 1, (int)mousePosY);
                        checkVariation((int)mousePosX - 1, (int)mousePosY);
                        checkVariation((int)mousePosX, (int)mousePosY + 1);
                        checkVariation((int)mousePosX, (int)mousePosY - 1);
                        checkVariation((int)mousePosX, (int)mousePosY);
                    }
                }*/
                int distance = 20;
                float rayX = 0;
                float rayY = 0;
                float rayDistance = 1f;
                for (int it = 0; it < distance; it++)
                {
                    rayX = (float)(Math.Cos(weoponDegree + Math.PI/2) * (it * rayDistance));
                    rayY = (float)(Math.Sin(weoponDegree + Math.PI/2) * (it * rayDistance*2f));
                    try
                    {
                        Console.Out.WriteLine("" + (rayY - stand.Height / 2));
                        if ((((characterBody.Position.Y * scale)) + rayY) > (((characterBody.Position.Y * scale)) - stand.Height/2))
                        {
                            if (terrain[(int)((((characterBody.WorldCenter.X * scale) - (stand.Width / 4)) + rayX) / scale), (int)((((characterBody.Position.Y * scale) - (stand.Height / 2)) + rayY) / scale)].getBlockType() != Block.NO_BLOCK)
                            {
                                terrain[(int)((((characterBody.WorldCenter.X * scale) - (stand.Width / 4)) + rayX) / scale), (int)((((characterBody.Position.Y * scale) - (stand.Height / 2)) + rayY) / scale)].setBlockType(Block.NO_BLOCK);
                                break;
                            }
                        }
                    }
                    catch (IndexOutOfRangeException e)
                    {
                    }
                }

            }
            else if (mouseStateCurrent.MiddleButton == ButtonState.Pressed && mouseStatePrevious.MiddleButton == ButtonState.Released)
            {
                if (mousePosX > 0 && mousePosX < xD && mousePosY > 0 && mousePosY < yD)
                {
                    if (terrain[(int)mousePosX, (int)mousePosY].getBlockType() == Block.NO_BLOCK)
                    {
                        terrain[(int)mousePosX, (int)mousePosY].setBlockType(Block.STONE_BLOCK);
                        terrain[(int)mousePosX, (int)mousePosY].setVariation(0);
                    }
                    else
                    {
                        terrain[(int)mousePosX, (int)mousePosY].setBlockType(Block.NO_BLOCK);
                        terrain[(int)mousePosX, (int)mousePosY].setVariation(0);
                    }
                    /*if (terrain[(int)mousePosX, (int)mousePosY].getBlockType() != Block.NO_BLOCK)
                        terrain[(int)mousePosX, (int)mousePosY].setBlockType(Block.NO_BLOCK);
                    else
                    {
                        terrain[(int)mousePosX, (int)mousePosY].setBlockType(Block.DIRT_BLOCK);
                        //checking for variation from mouse click
                        checkVariation((int)mousePosX + 1, (int)mousePosY);
                        checkVariation((int)mousePosX - 1, (int)mousePosY);
                        checkVariation((int)mousePosX, (int)mousePosY + 1);
                        checkVariation((int)mousePosX, (int)mousePosY - 1);
                        checkVariation((int)mousePosX, (int)mousePosY);
                    }*/
                }
            }
            else if (mouseStateCurrent.RightButton == ButtonState.Pressed && mouseStatePrevious.RightButton == ButtonState.Released)
            {
                circleBody = BodyFactory.CreateCircle(world, 15f / (2f * scale), 1f, new Vector2(mousePosX, mousePosY));
                circleBody.BodyType = BodyType.Dynamic;

                // Give it some bounce and friction
                circleBody.Restitution = 0.0f;
                circleBody.Friction = 0.2f;
                objects.Add(circleBody);
            }

            //weoponDegree += 1f;
            if (strafeMode == true)
            {
                weoponDegree = -(float)Math.Atan2(xPos - WINDOW_WIDTH / 2, yPos - WINDOW_HEIGHT / 2);
            }
            else
            {
                if (direction == 1)
                    weoponDegree = MathHelper.ToRadians(180);
                else
                    weoponDegree = MathHelper.ToRadians(0);
                //weoponDegree = 90 * direction;
            }
            //weoponDegree = (float)Math.Atan2(WINDOW_WIDTH / 2 - xPos, WINDOW_HEIGHT / 2 - yPos);
            if (strafeMode == true)
            {
                float rad = (weoponDegree * (float)Math.PI / 180);
                weoponPos = new Vector2((float)(Math.Cos(rad) * 100 + (GraphicsDevice.Viewport.Width / 2)) / GraphicsDevice.Viewport.Width, (float)(Math.Sin(rad) * 100 + (GraphicsDevice.Viewport.Height / 2)) / GraphicsDevice.Viewport.Height);
            }
            else
            {
                float rad = (weoponDegree * (float)Math.PI / 180);
                weoponPos = new Vector2((float)(Math.Cos(rad) * 100 + (GraphicsDevice.Viewport.Width / 2)) / GraphicsDevice.Viewport.Width, (float)(Math.Sin(rad) * 100 + (GraphicsDevice.Viewport.Height / 2)) / GraphicsDevice.Viewport.Height);

            }
            if (!strafeMode)
            {
                if (MathHelper.ToDegrees(weoponDegree) >= 0 && MathHelper.ToDegrees(weoponDegree) < 180)
                    direction = -1;
                else
                    direction = 1;
            }
            mouseStatePrevious = mouseStateCurrent;
        }
        #endregion
        #region "input"
        private void HandleInput()
        {
            GamePadState padState = GamePad.GetState(0);
            KeyboardState state = Keyboard.GetState();

            /*weoponDegree = (float)Math.Atan2((WINDOW_WIDTH/2 + padState.ThumbSticks.Right.X) - WINDOW_WIDTH / 2, (WINDOW_HEIGHT/2 + padState.ThumbSticks.Right.Y) - WINDOW_HEIGHT / 2) + MathHelper.ToRadians(180);
            //weoponDegree = (float)Math.Atan2(WINDOW_WIDTH / 2 - xPos, WINDOW_HEIGHT / 2 - yPos);
            float rad = (weoponDegree * (float)Math.PI / 180);
            weoponPos = new Vector2((float)(Math.Cos(rad) * 100 + (GraphicsDevice.Viewport.Width / 2)) / GraphicsDevice.Viewport.Width, (float)(Math.Sin(rad) * 100 + (GraphicsDevice.Viewport.Height / 2)) / GraphicsDevice.Viewport.Height);*/

            /*if (!strafeMode)
            {
                if (MathHelper.ToDegrees(weoponDegree) >= 0 && MathHelper.ToDegrees(weoponDegree) < 180)
                    direction = -1;
                else
                    direction = 1;
            }*/
            #region "gameEnabled"
            if (!menuEnabled)
            {
                if (padState.Buttons.RightShoulder == ButtonState.Pressed)
                {
                    strafeMode = true;
                }
                else
                    strafeMode = false;
                if (state.IsKeyDown(Keys.LeftControl))
                {
                    zoom += .01f;
                }
                if (state.IsKeyDown(Keys.LeftAlt))
                {
                    zoom -= .01f;
                }
                if ((state.IsKeyDown(Keys.Q)))
                {
                    strafeMode = true;
                    usingArm = true;
                    weoponDegree = -(float)Math.Atan2(xPos - WINDOW_WIDTH / 2, yPos - WINDOW_HEIGHT / 2);
                }
                else
                {
                    usingArm = false;
                    strafeMode = false;
                }
                if ((state.IsKeyDown(Keys.W) && _touchingFloor == true) || ((padState.IsButtonDown(Buttons.RightTrigger) || padState.IsButtonDown(Buttons.A)) && _touchingFloor == true))
                {
                    if (jumpCooldown == JUMP_COOLDOWN)
                    {
                        characterBody.LinearVelocity = new Vector2(characterBody.LinearVelocity.X, -14);
                        jumpCooldown--;
                    }
                }
                if (state.IsKeyDown(Keys.A) || padState.ThumbSticks.Left.X < 0)
                {
                    if (!strafeMode)
                    {
                        if (!usingArm)
                            direction = -1;
                    }
                    cooldown--;
                    if (cooldown <= 0)
                    {
                        change = !change;
                        cooldown = COOLDOWN;
                    }
                    if (padState.ThumbSticks.Left.X < 0)
                    {
                        if (_touchingFloor)
                        {
                            characterBody.ApplyLinearImpulse(new Vector2(-1.5f * Math.Abs(padState.ThumbSticks.Left.X), 0));
                        }
                        else
                        {
                            characterBody.ApplyLinearImpulse(new Vector2(-1f * Math.Abs(padState.ThumbSticks.Left.X), 0));
                        }
                    }
                    else
                    {
                        if (_touchingFloor)
                        {
                            characterBody.ApplyLinearImpulse(new Vector2(-1.5f, 0));
                        }
                        else
                        {
                            characterBody.ApplyLinearImpulse(new Vector2(-1f, 0));
                        }
                    }
                    if (characterBody.LinearVelocity.X < -maxSpeed)
                        characterBody.LinearVelocity = new Vector2(-maxSpeed, characterBody.LinearVelocity.Y);
                }
                else if (state.IsKeyDown(Keys.D) || padState.ThumbSticks.Left.X > 0)
                {
                    if (!strafeMode)
                    {
                        if (!usingArm)
                            direction = 1;
                    }
                    cooldown--;
                    if (cooldown <= 0)
                    {
                        change = !change;
                        cooldown = COOLDOWN;
                    }
                    if (padState.ThumbSticks.Left.X > 0)
                    {
                        if (_touchingFloor)
                        {
                            characterBody.ApplyLinearImpulse(new Vector2(1.5f * Math.Abs(padState.ThumbSticks.Left.X), 0));
                        }
                        else
                        {
                            characterBody.ApplyLinearImpulse(new Vector2(1f * Math.Abs(padState.ThumbSticks.Left.X), 0));
                        }
                    }
                    else
                    {
                        if (_touchingFloor)
                        {
                            characterBody.ApplyLinearImpulse(new Vector2(1.5f, 0));
                        }
                        else
                        {
                            characterBody.ApplyLinearImpulse(new Vector2(1f, 0));
                        }
                    }

                    if (characterBody.LinearVelocity.X > maxSpeed)
                        characterBody.LinearVelocity = new Vector2(maxSpeed, characterBody.LinearVelocity.Y);
                }
                else if (!_touchingFloor)
                {
                    characterBody.ApplyLinearImpulse(new Vector2(-characterBody.LinearVelocity.X * .6f, 0));
                }
                else
                {
                    change = false;
                    characterBody.ApplyLinearImpulse(new Vector2(-characterBody.LinearVelocity.X * .6f, 0));
                }

                if (Keyboard.GetState().IsKeyDown(Keys.F1))
                    rays.lightTexture = Content.Load<Texture2D>("Textures/flare");
                if (Keyboard.GetState().IsKeyDown(Keys.F2))
                    rays.lightTexture = Content.Load<Texture2D>("Textures/flare2");
                if (Keyboard.GetState().IsKeyDown(Keys.F3))
                    rays.lightTexture = Content.Load<Texture2D>("Textures/flare3");
                if (Keyboard.GetState().IsKeyDown(Keys.F4))
                    rays.lightTexture = Content.Load<Texture2D>("Textures/flare4");

                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    charmNum += 0.1f;
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    charmNum -= 0.1f;
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    charmNum += 0.01f;
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    charmNum -= 0.01f;
            }
            #endregion
            #region "menuEnabled"
            else
            {
                if ((state.IsKeyDown(Keys.W) && oldKeyState.IsKeyUp(Keys.W)) || (state.IsKeyDown(Keys.Up) && oldKeyState.IsKeyUp(Keys.Up)))
                {
                    menuBlip1.Play();
                    menuNum--;
                    if (menuNum <= 0)
                    {
                        menuNum = 3;
                    }
                }
                if ((state.IsKeyDown(Keys.S) && oldKeyState.IsKeyUp(Keys.S)) || (state.IsKeyDown(Keys.Down) && oldKeyState.IsKeyUp(Keys.Down)))
                {
                    menuBlip1.Play();
                    menuNum++;
                    if (menuNum >= 4)
                    {
                        menuNum = 1;
                    }
                }
                if (state.IsKeyDown(Keys.Enter) && oldKeyState.IsKeyUp(Keys.Enter))
                {
                    menuBlip2.Play();
                    if (menuNum == 1)
                    {
                        fadeOut = true;
                    }
                }
                if (padState.ThumbSticks.Left.Y > 0.1f || padState.DPad.Up == ButtonState.Pressed)
                {
                    if (!resetMenuButtonController)
                    {
                        menuBlip1.Play();
                        menuNum--;
                        if (menuNum <= 0)
                        {
                            menuNum = 3;
                        }
                    }
                    resetMenuButtonController = true;
                }
                else if (padState.ThumbSticks.Left.Y < -0.1f || padState.DPad.Down == ButtonState.Pressed)
                {
                    if (!resetMenuButtonController)
                    {
                        menuBlip1.Play();
                        menuNum++;
                        if (menuNum >= 4)
                        {
                            menuNum = 1;
                        }
                    }
                    resetMenuButtonController = true;
                }
                else
                {
                    resetMenuButtonController = false;
                }
                if (padState.Buttons.A == ButtonState.Pressed && oldPadState.Buttons.A == ButtonState.Released)
                {
                    menuBlip2.Play();
                    if (menuNum == 1)
                    {
                        fadeOut = true;
                    }
                }
            }
            #endregion
            #region "globalInput"
            if (state.IsKeyDown(Keys.B) && oldKeyState.IsKeyUp(Keys.B))
            {
                debug = !debug;
            }
            if (state.IsKeyDown(Keys.C) && oldKeyState.IsKeyUp(Keys.C))
            {
                for (int zi = 0; zi < objects.Count; zi++)
                {
                    world.RemoveBody(objects[zi]);
                }
                objects.Clear();
                _fluidSimulation.emptyParticles();
            }
            if (state.IsKeyDown(Keys.F) && oldKeyState.IsKeyUp(Keys.F))
            {
                useFXAA = !useFXAA;
            }
            if (state.IsKeyDown(Keys.Escape) || padState.IsButtonDown(Buttons.Back))
                Exit();
            #endregion
            oldKeyState = state;
            oldPadState = padState;
        }
        #endregion
        #region "draw"
        protected override void Draw(GameTime gameTime)
        {
            water.DrawToRenderTargets();
            graphics.GraphicsDevice.SetRenderTarget(scene);
            graphics.GraphicsDevice.Clear(Color.Transparent);
            drawStuff(frontTime);
            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            batch.Draw(sceneRegPre, Vector2.Zero, new Color(blendAmount, blendAmount, blendAmount, blendAmount));
            batch.End();
            graphics.GraphicsDevice.SetRenderTarget(null);

            graphics.GraphicsDevice.SetRenderTarget(sceneRegB);
            graphics.GraphicsDevice.Clear(Color.Transparent);
            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            //batch.Draw(scene, new Vector2(-2, -2), new Color(0.4f, 0.4f, 0.4f, 0.4f));
            batch.Draw(scene, -new Vector2((float)Math.Cos(sunRad), (float)Math.Sin(sunRad))*5, new Color(0f, 0f, 0.0f, shadowAmount));
            batch.Draw(scene, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), Color.White);
            //spriteBatch.Draw(particlesTarget, Vector2.One, new Color(0f, 0f, 0.2f, 0.5f));
            //batch.Draw(scene, Vector2.One, new Color(0f, 0f, 0.2f, 0.5f));
            batch.End();

            graphics.GraphicsDevice.SetRenderTarget(null);

            graphics.GraphicsDevice.SetRenderTarget(sceneRegPre);
            graphics.GraphicsDevice.Clear(Color.Transparent);
            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            batch.Draw(scene, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), Color.White);
            batch.End();

            graphics.GraphicsDevice.SetRenderTarget(null);

            graphics.GraphicsDevice.SetRenderTarget(sceneReg);
            graphics.GraphicsDevice.Clear(Color.White);
            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);            
            batch.Draw(scene, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), Color.Black);
            batch.End();

            graphics.GraphicsDevice.SetRenderTarget(null);

            ppm.Draw(gameTime, sceneReg);

            graphics.GraphicsDevice.SetRenderTarget(scene);
            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            //background
            batch.Draw(blissSprite, new Vector2(0, 0), backgroundRectangle, dayTime);
            batch.Draw(sceneRegB, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), Color.White);
            batch.End();

            graphics.GraphicsDevice.SetRenderTarget(null);

            graphics.GraphicsDevice.SetRenderTarget(sceneReg);

            graphics.GraphicsDevice.Clear(Color.Transparent);

            batch.Begin(SpriteSortMode.Immediate, BlendState.Additive);            
            batch.Draw(ppm.Scene, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), Color.White);
            batch.Draw(scene, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), Color.White);
            batch.End();

            graphics.GraphicsDevice.SetRenderTarget(scene);
            graphics.GraphicsDevice.Clear(Color.Transparent);
            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            //batch.Draw(blissSprite, new Vector2(0, 0), backgroundRectangle, Color.Wheat);
            batch.Draw(sceneReg, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), Color.White);
            batch.Draw(vignette, new Vector2(0, 0), null, Color.White, 0f, new Vector2(0, 0), (float)WINDOW_WIDTH / vignette.Width, SpriteEffects.None, 0f);
            batch.End();
            
            cam._pos = -cam._pos;

            Matrix projection = Matrix.CreateOrthographicOffCenter(0f, (worldPosition2.X - worldPosition1.X) / scale, (worldPosition2.Y - worldPosition1.Y) / scale, 0f, 0f, 1f);
            Matrix view = Matrix.CreateTranslation(new Vector3((cam._pos / scale) + new Vector2((worldPosition2.X - worldPosition1.X) / 2, (worldPosition2.Y - worldPosition1.Y) / 2) / scale, 0f));

            if (debug)
            {
                batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);                
                batch.DrawString(font, "" + _touchingFloor, new Vector2(12f, 12f), Color.White);
                batch.DrawString(font, "" + drawAmount.ToString(), new Vector2(14f, 148), Color.Black);
                batch.DrawString(font, "" + rays.lightSource, new Vector2(14f, 268), Color.Black);
                batch.DrawString(font, "" + weoponPos, new Vector2(14f, 388), Color.Black);
                batch.DrawString(font, "" + weoponDegree, new Vector2(14f, 408), Color.Black);
                batch.DrawString(font, "" + (MathHelper.ToDegrees(weoponDegree)), new Vector2(14f, 428), Color.Black);
                batch.DrawString(font, "" + 1 / cam.Zoom, new Vector2(14f, 448), Color.Black);

                batch.DrawString(font, "" + MathHelper.ToDegrees(weoponDegree+3.14f), new Vector2(54f, 648), Color.Black);

                batch.DrawString(font, "" + (cam._pos), new Vector2(14f, 488), Color.Red);
                batch.DrawString(font, "" + worldPosition, new Vector2(14f, 508), Color.Red);
                //batch.DrawString(font, "" + (MathHelper.ToDegrees(armBody.Rotation)), new Vector2(14f, 448), Color.Black);f

                batch.End();
                _debugView.RenderDebugData(ref projection, ref view);
            }

            cam._pos = -cam._pos;
            _view = Matrix.CreateTranslation(new Vector3(cam._pos - _screenCenter, 0f)) * Matrix.CreateTranslation(new Vector3(_screenCenter, 0f));

            Viewport viewport = GraphicsDevice.Viewport;
            Matrix realProjection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            fxaaEffect.Parameters["World"].SetValue(Matrix.Identity);
            fxaaEffect.Parameters["View"].SetValue(Matrix.Identity);
            fxaaEffect.Parameters["Projection"].SetValue(halfPixelOffset * realProjection);
            fxaaEffect.Parameters["InverseViewportSize"].SetValue(new Vector2(1f / viewport.Width, 1f / viewport.Height));
            fxaaEffect.Parameters["ConsoleSharpness"].SetValue(new Vector4(
                -N / viewport.Width,
                -N / viewport.Height,
                N / viewport.Width,
                N / viewport.Height
                ));
            fxaaEffect.Parameters["ConsoleOpt1"].SetValue(new Vector4(
                -2.0f / viewport.Width,
                -2.0f / viewport.Height,
                2.0f / viewport.Width,
                2.0f / viewport.Height
                ));
            fxaaEffect.Parameters["ConsoleOpt2"].SetValue(new Vector4(
                8.0f / viewport.Width,
                8.0f / viewport.Height,
                -4.0f / viewport.Width,
                -4.0f / viewport.Height
                ));
            fxaaEffect.Parameters["SubPixelAliasingRemoval"].SetValue(subPixelAliasingRemoval);
            fxaaEffect.Parameters["EdgeThreshold"].SetValue(edgeTheshold);
            fxaaEffect.Parameters["EdgeThresholdMin"].SetValue(edgeThesholdMin);
            fxaaEffect.Parameters["ConsoleEdgeSharpness"].SetValue(consoleEdgeSharpness);
            fxaaEffect.Parameters["ConsoleEdgeThreshold"].SetValue(consoleEdgeThreshold);
            fxaaEffect.Parameters["ConsoleEdgeThresholdMin"].SetValue(consoleEdgeThresholdMin);

            fxaaEffect.CurrentTechnique = fxaaEffect.Techniques[useFXAA ? "FXAA" : "Standard"];

            GraphicsDevice.SetRenderTarget(null);
            bloom.BeginDraw();
            batch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.LinearClamp, null, null, fxaaEffect);
            batch.Draw(scene, Vector2.Zero, Color.White);
            batch.End();

            base.Draw(gameTime);
        }
        #region "drawClouds"
        public static void DrawRectangle(Rectangle coords, Color color)
        {
            //Texture2D result = gaussianBlur.PerformGaussianBlur(textures[10], renderTarget1, renderTarget2, batch);
            for (int y = 1; y < (coords.Height / 16); y++)
            {
                //batch.End();

                Rectangle rectangle = new Rectangle(coords.X, coords.Y + (y * 16), 16, 16);

                //graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

                batch.Begin();
                //batch.Draw((Texture2D)textures[10], new Vector2(coords.X, coords.Y + (y * 16)), new Rectangle(0, 0, (int)scale, (int)scale), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                if (blur)
                    batch.Draw(result, rectangle, Color.White);
                batch.Draw((Texture2D)textures[10], new Vector2(coords.X, coords.Y + (y * 16)), new Rectangle(0, 0, (int)scale, (int)scale), color, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                batch.End();
                //batch.Draw((Texture2D)textures[10], new Vector2(coords.X, coords.Y + (y * 16)), new Rectangle(0, 0, (int)scale, (int)scale), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            }
            //rect = new Texture2D(graphics.GraphicsDevice, 1, 1);
            //rect.SetData(new[] { color });
            //batch.Draw(rect, coords, color);
        }
        #endregion
        #endregion
        #region "drawStuff"
        public void drawStuff(Color color)
        {
            /*batch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null);
            {
                batch.Draw(back, new Vector2(0, WINDOW_HEIGHT - back.Height*1.2f), new Rectangle((int)(cam._pos.X * 0.04f), (int)(0), back.Width, back.Height), color);
                batch.Draw(back, new Vector2(0, WINDOW_HEIGHT - back.Height*1.1f), new Rectangle((int)(cam._pos.X * 0.08f), (int)(0), back.Width, back.Height), color);
                batch.Draw(back, new Vector2(0, WINDOW_HEIGHT - back.Height), new Rectangle((int)(cam._pos.X * 0.1f), (int)(0), back.Width, back.Height), color);
                //batch.Draw(back, new Vector2(0, 0), new Rectangle((int)(cam._pos.X * 0.5f), (int)(cam._pos.Y * 0.5f), back.Width, back.Height), Color.White);
                //batch.Draw(back, new Vector2(0, 0), new Rectangle((int)(cam._pos.X * 0.8f), (int)(cam._pos.Y * 0.8f), back.Width, back.Height), Color.White);
                //batch.Draw(back, new Vector2(0, 0), new Rectangle((int)(cam._pos.X * 1.0f), (int)(cam._pos.Y * 1.0f), back.Width, back.Height), Color.White);
            }
            batch.End();*/
            /*batch.Begin();
            {
                batch.Draw(back, new Vector2(0, 0), null, color, 0f, new Vector2(0 / 2f, 0 / 2f), 1f, SpriteEffects.None, 0f);
                batch.Draw(back, new Vector2(0, 0), null, color, 0f, new Vector2(0 / 2f, 0 / 2f), 1f, SpriteEffects.None, 0f);
                batch.Draw(back, new Vector2(0, 0), null, color, 0f, new Vector2(0 / 2f, 0 / 2f), 1f, SpriteEffects.None, 0f);
            }
            batch.End();*/
            if (blur)
                result = gaussianBlur.PerformGaussianBlur(textures[10], renderTarget1, renderTarget2, batch);
            for (int cloudNum = 0; cloudNum < clouds.Count; cloudNum++)
            {
                clouds[cloudNum].draw(color);
            }

            //draw fluids
            water.Draw(color);

            batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, cam.get_transformation(graphics.GraphicsDevice));
            //Draw circle
            if (direction == 1)
            {
                if (change)
                    batch.Draw(walk, characterBody.Position * scale, null, color, 0f, new Vector2((14 / 2f), 28 / 2f), 1f, SpriteEffects.None, 0f);
                else
                    batch.Draw(stand, characterBody.Position * scale, null, color, 0f, new Vector2((14 / 2f), 28 / 2f), 1f, SpriteEffects.None, 0f);
            }
            else
            {
                if (change)
                    batch.Draw(walk, characterBody.Position * scale, null, color, 0f, new Vector2((14 / 2f), 28 / 2f), 1f, SpriteEffects.FlipHorizontally, 0f);
                else
                    batch.Draw(stand, characterBody.Position * scale, null, color, 0f, new Vector2((14 / 2f), 28 / 2f), 1f, SpriteEffects.FlipHorizontally, 0f);
            }
            if (direction == 1)
            {
                if (strafeMode)
                {
                    //batch.Draw(sword, characterBody.Position * scale + new Vector2((6 / 2f), -5 / 2f), null, color, weoponDegree, new Vector2(3, -8), 1f, SpriteEffects.None, 0f);
                    batch.Draw(pick, characterBody.Position * scale + new Vector2((6 / 2f), -5 / 2f), null, color, weoponDegree, new Vector2(pick.Width / 2f, -8), 1f, SpriteEffects.None, 0f);
                    batch.Draw(arm1, characterBody.Position * scale + new Vector2((6 / 2f), -5 / 2f), null, color, weoponDegree, new Vector2(1, 0), 1f, SpriteEffects.None, 0f);
                }
                else
                {
                    //batch.Draw(sword, characterBody.Position * scale + new Vector2((26 / 2f), 5 / 2f), null, color, -MathHelper.ToRadians(110), new Vector2(sword.Width / 2, sword.Height / 2), 1f, SpriteEffects.None, 0f);
                    batch.Draw(pick, characterBody.Position * scale + new Vector2(((22+pick.Width/2f) / 2f), 2 / 2f), null, color, -MathHelper.ToRadians(110), new Vector2(pick.Width / 2, pick.Height / 2), 1f, SpriteEffects.None, 0f);
                    batch.Draw(arm1, characterBody.Position * scale + new Vector2((10 / 2f), -6 / 2f + arm1.Height), null, color, weoponDegree, new Vector2(1, 0), 1f, SpriteEffects.None, 0f);
                }
            }
            else
            {
                if (strafeMode)
                {
                    //batch.Draw(sword, characterBody.Position * scale + new Vector2((-6 / 2f), -5 / 2f), null, color, weoponDegree, new Vector2(3, -8), 1f, SpriteEffects.None, 0f);
                    batch.Draw(pick, characterBody.Position * scale + new Vector2((-6 / 2f), -5 / 2f), null, color, weoponDegree, new Vector2(pick.Width / 2f, -8), 1f, SpriteEffects.None, 0f);
                    batch.Draw(arm1, characterBody.Position * scale + new Vector2((-6 / 2f), -5 / 2f), null, color, weoponDegree, new Vector2(1, 0), 1f, SpriteEffects.None, 0f);
                }
                else
                {
                    //batch.Draw(sword, characterBody.Position * scale + new Vector2((-26 / 2f), 5 / 2f), null, color, MathHelper.ToRadians(110), new Vector2(sword.Width / 2, sword.Height / 2), 1f, SpriteEffects.None, 0f);
                    batch.Draw(pick, characterBody.Position * scale + new Vector2(((-22 - pick.Width/2f) / 2f),2 / 2f), null, color, MathHelper.ToRadians(110), new Vector2(pick.Width / 2, pick.Height / 2), 1f, SpriteEffects.None, 0f);
                    batch.Draw(arm1, characterBody.Position * scale + new Vector2((-10 / 2f), -6 / 2f), null, color, weoponDegree, new Vector2(1, 0), 1f, SpriteEffects.None, 0f);
                }
            }
            for (int zi = 0; zi < objects.Count; zi++)
            {
                Vector2 circlePos = objects[zi].Position * scale;
                float circleRotation = objects[zi].Rotation;
                Vector2 circleOrigin = new Vector2(circleSprite.Width / 2f, circleSprite.Height / 2f);

                batch.Draw(circleSprite, circlePos, null, color, circleRotation, circleOrigin, 1f, SpriteEffects.None, 0f);
            }
            drawBlocks(color);
            batch.End();
            if (debug)
            {
                _fluidSimulation.draw();
            }
            if (menuEnabled)
            {
                batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                batch.DrawString(bitWonder, "BitSmith", new Vector2(3 + WINDOW_WIDTH / 2 - bitWonder.MeasureString("BitSmith").X / 2, 3 + WINDOW_HEIGHT / 6), Color.Black);
                //batch.DrawString(bitWonder, "BitSmith", new Vector2(2 + WINDOW_WIDTH / 2 - bitWonder.MeasureString("BitSmith").X / 2, 2 - WINDOW_HEIGHT / 6), Color.Black);
                //batch.DrawString(bitWonder, "BitSmith", new Vector2(2 - WINDOW_WIDTH / 2 - bitWonder.MeasureString("BitSmith").X / 2, 2 + WINDOW_HEIGHT / 6), Color.Black);
                //batch.DrawString(bitWonder, "BitSmith", new Vector2(2 - WINDOW_WIDTH / 2 - bitWonder.MeasureString("BitSmith").X / 2, 2 - WINDOW_HEIGHT / 6), Color.Black);

                batch.DrawString(bitWonder, "BitSmith", new Vector2(WINDOW_WIDTH / 2 - bitWonder.MeasureString("BitSmith").X / 2, WINDOW_HEIGHT / 6), Color.LightGray);

                batch.DrawString(minecraftia, "New Game", new Vector2(2 + WINDOW_WIDTH / 2 - minecraftia.MeasureString("New Game").X / 2, 2 + (WINDOW_HEIGHT / 5) * 2), Color.Black);
                batch.DrawString(minecraftia, "Achievements", new Vector2(2 + WINDOW_WIDTH / 2 - minecraftia.MeasureString("Achievements").X / 2, 2 + (WINDOW_HEIGHT / 5) * 2.5f), Color.Black);
                batch.DrawString(minecraftia, "Unlock Game", new Vector2(2 + WINDOW_WIDTH / 2 - minecraftia.MeasureString("Unlock Game").X / 2, 2 + (WINDOW_HEIGHT / 5) * 3), Color.Black);

                //dark goldenrond
                if (menuNum == 1)
                {
                    batch.DrawString(minecraftia, "New Game", new Vector2(WINDOW_WIDTH / 2 - minecraftia.MeasureString("New Game").X / 2, (WINDOW_HEIGHT / 5) * 2), Color.DarkGoldenrod);
                }
                else
                {
                    batch.DrawString(minecraftia, "New Game", new Vector2(WINDOW_WIDTH / 2 - minecraftia.MeasureString("New Game").X / 2, (WINDOW_HEIGHT / 5) * 2), Color.LightGray);
                }

                if (menuNum == 2)
                {
                    batch.DrawString(minecraftia, "Achievements", new Vector2(WINDOW_WIDTH / 2 - minecraftia.MeasureString("Achievements").X / 2, (WINDOW_HEIGHT / 5) * 2.5f), Color.DarkGoldenrod);
                }
                else
                {
                    batch.DrawString(minecraftia, "Achievements", new Vector2(WINDOW_WIDTH / 2 - minecraftia.MeasureString("Achievements").X / 2, (WINDOW_HEIGHT / 5) * 2.5f), Color.LightGray);
                }

                if (menuNum == 3)
                {
                    batch.DrawString(minecraftia, "Unlock Game", new Vector2(WINDOW_WIDTH / 2 - minecraftia.MeasureString("Unlock Game").X / 2, (WINDOW_HEIGHT / 5) * 3), Color.DarkGoldenrod);
                }
                else
                {
                    batch.DrawString(minecraftia, "Unlock Game", new Vector2(WINDOW_WIDTH / 2 - minecraftia.MeasureString("Unlock Game").X / 2, (WINDOW_HEIGHT / 5) * 3), Color.LightGray);
                }
                batch.End();
            }

            if (fadeOut)
            {
                Texture2D texture = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                texture.SetData<Color>(new Color[] { Color.White });

                batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                batch.Draw(texture, new Rectangle(0, 0, WINDOW_WIDTH, WINDOW_HEIGHT), new Color(0,0,0,fadeOutNum));
                batch.End();
            }
        }
        #endregion
        #region "drawBlocks"
        public void drawBlocks(Color color)
        {
            drawAmount = 0;
            worldPosition1 = Vector2.Transform(new Vector2(0, 0), Matrix.Invert(cam.get_transformation(graphics.GraphicsDevice)));
            worldPosition2 = Vector2.Transform(new Vector2(WINDOW_WIDTH, WINDOW_HEIGHT), Matrix.Invert(cam.get_transformation(graphics.GraphicsDevice)));
            int xD1 = (int)((worldPosition1.X) / 16);
            if (xD1 < 0)
                xD1 = 0;
            if (xD1 > xD)
                xD1 = xD;
            int yD1 = (int)((worldPosition1.Y) / 16);
            if (yD1 < 0)
                yD1 = 0;
            if (yD1 > yD)
                yD1 = yD;
            int xD2 = (int)(((worldPosition2.X) / 16) + 1);
            if (xD2 < 0)
                xD2 = 0;
            if (xD2 > xD)
                xD2 = xD;
            int yD2 = (int)(((worldPosition2.Y) / 16) + 1);
            if (yD2 < 0)
                yD2 = 0;
            if (yD2 > yD)
                yD2 = yD;

            for (int xi = xD1; xi < xD2; xi++)
            {
                for (int yi = yD1; yi < yD2; yi++)
                {
                    drawAmount++;
                    //graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap; 
                    if (terrain[xi, yi].getBlockType() == Block.NO_BLOCK)
                        continue;
                    else if (terrain[xi, yi].getVariation() != 0)
                        batch.Draw((Texture2D)grass[terrain[xi, yi].getVariation()], new Vector2(xi * scale, yi * scale), new Rectangle(0, 0, (int)scale, (int)scale), color, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                    else
                        batch.Draw((Texture2D)textures[terrain[xi, yi].getBlockType()], new Vector2(xi * scale, yi * scale), new Rectangle(0, 0, (int)scale, (int)scale), color, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                }
            }
        }
        #endregion
        #region "checker"
        private bool CharacterOnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            _collision = true;
            return true;
        }
        private void CharacterOnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
            _collision = false;
        }
        public static void checkGridLiquid(Vector2 _position)
        {
            float x = _position.X;
            float y = _position.Y;
            //int row = (int)(x + (objects[zi].LinearVelocity.X / (scale * 2)));
            //int col = (int)(y + (objects[zi].LinearVelocity.Y / (scale * 2)));
            int row = (int)(x);
            int col = (int)(y);
            for (int xP = -1; xP < 2; xP++)
            {
                for (int yP = -1; yP < 2; yP++)
                {
                    if (row > 1 && col > 1 && row < xD - 2 && col < yD - 2)
                    {
                        if (terrain[row + xP, col + yP].getBlockType() != Block.NO_BLOCK && terrain[row + xP, col + yP].getBlockType() != Block.GRASS_BLOCK)
                        {
                            if (terrain[row + xP, col + yP].getBlockBlody() == null)
                            {
                                terrain[row + xP, col + yP].createBlock(new Vector2(row + xP, col + yP));
                                deleteBlocks.Add(new Vector2(row + xP, col + yP));
                            }
                            else
                                terrain[row + xP, col + yP].setDelete(false);
                        }
                    }
                }

            }
        }
        public static bool checkParticle(Vector2 _position)
        {
            float x = _position.X;
            float y = _position.Y;
            int row = (int)(x);
            int col = (int)(y);
            if (row > 1 && col > 1 && row < xD - 2 && col < yD - 2)
            {
                if (terrain[row, col].getBlockType() == Block.NO_BLOCK)
                {
                    return false;
                }
                else if (terrain[row, col].getBlockType() != Block.GRASS_BLOCK)
                {
                    return true;
                }
            }
            return false;
        }
        public static void checkGrid(Vector2 _position)
        {
            float x = _position.X;
            float y = _position.Y;
            //int row = (int)(x + (objects[zi].LinearVelocity.X / (scale * 2)));
            //int col = (int)(y + (objects[zi].LinearVelocity.Y / (scale * 2)));
            int row = (int)(x);
            int col = (int)(y);
            for (int xP = -1; xP < 2; xP++)
            {
                for (int yP = -1; yP < 2; yP++)
                {
                    if (row > 1 && col > 1 && row < xD - 2 && col < yD - 2)
                    {
                        if (terrain[row + xP, col + yP].getBlockType() != Block.NO_BLOCK && terrain[row + xP, col + yP].getBlockType() != Block.GRASS_BLOCK)
                        {
                            if (terrain[row + xP, col + yP].getBlockBlody() == null)
                            {
                                terrain[row + xP, col + yP].createBlock(new Vector2(row + xP, col + yP));
                                deleteBlocks.Add(new Vector2(row + xP, col + yP));
                            }
                            else
                                terrain[row + xP, col + yP].setDelete(false);
                        }
                    }
                }
            }
        }
        public static Body CreateCapsule(World world, float height, float endRadius, float density,object userData)
        {
            //Create the middle rectangle
            Vertices rectangle = PolygonTools.CreateRectangle(endRadius * 0.95f, height / 2);

            List<Vertices> list = new List<Vertices>();
            list.Add(rectangle);

            Body body = FarseerPhysics.Factories.BodyFactory.CreateCompoundPolygon(world, list, density, userData);

            //Create the two circles
            CircleShape topCircle = new CircleShape(endRadius, density);
            topCircle.Position = new Vector2(0, height / 2);
            body.CreateFixture(topCircle, userData);

            CircleShape bottomCircle = new CircleShape(endRadius, density);
            bottomCircle.Position = new Vector2(0, -(height / 2));
            body.CreateFixture(bottomCircle, userData);
            return body;
        }
        #endregion
        #region "terrain"
        public static Block[,] getTerrain()
        {
            return terrain;
        }
        public static void checkVariation(int xi, int yi)
        {
            if (terrain[xi - 1, yi].getBlockType() == 0)
            {
                if (terrain[xi, yi - 1].getBlockType() == 0)
                {
                    if (terrain[xi + 1, yi].getBlockType() == 0)
                    {
                        terrain[xi, yi].setVariation(7);
                    }
                    else
                    {
                        terrain[xi, yi].setVariation(4);
                    }
                }
                else if (terrain[xi + 1, yi].getBlockType() == 0)
                {
                    //leftright
                    terrain[xi, yi].setVariation(6);
                }
                else
                {
                    //left
                    if (terrain[xi - 1, yi - 1].getBlockType() == 0)
                    {
                        if (terrain[xi, yi - 1].getVariation() == 1 || terrain[xi, yi - 1].getVariation() == 4)
                        {
                            terrain[xi, yi].setVariation(1);
                        }
                    }
                    terrain[xi, yi].setVariation(1);
                }
            }
            else if (terrain[xi, yi - 1].getBlockType() == 0)
            {
                if (terrain[xi - 1, yi].getBlockType() == 0)
                {
                    if (terrain[xi + 1, yi].getBlockType() == 0)
                    {
                        terrain[xi, yi].setVariation(7);
                    }
                    else
                    {
                        terrain[xi, yi].setVariation(4);
                    }
                }
                else if (terrain[xi + 1, yi].getBlockType() == 0)
                {
                    terrain[xi, yi].setVariation(5);
                }
                else
                {
                    terrain[xi, yi].setVariation(2);
                }
            }
            else if (terrain[xi + 1, yi].getBlockType() == 0)
            {
                if (terrain[xi, yi - 1].getBlockType() == 0)
                {
                    if (terrain[xi - 1, yi].getBlockType() == 0)
                    {
                        terrain[xi, yi].setVariation(7);
                    }
                    else
                    {
                        terrain[xi, yi].setVariation(5);
                    }
                }
                else if (terrain[xi - 1, yi].getBlockType() == 0)
                {
                    //leftright
                    terrain[xi, yi].setVariation(6);
                }
                else
                {
                    //right
                    if (terrain[xi + 1, yi - 1].getBlockType() == 0)
                    {
                        if (terrain[xi, yi - 1].getVariation() == 3 || terrain[xi, yi - 1].getVariation() == 5)
                        {
                            terrain[xi, yi].setVariation(3);
                        }
                    }
                    terrain[xi, yi].setVariation(3);
                }
            }
            else
            {
                terrain[xi, yi].setVariation(0);
            }
        }
        public static void checkGrass(int xi, int yi)
        {
            if (terrain[xi, yi].getVariation() == 2 || terrain[xi, yi].getVariation() == 4 || terrain[xi, yi].getVariation() == 5 || terrain[xi, yi].getVariation() == 7)
            {
                int r = random.Next(0,50);
                if (r < 25)
                {
                    terrain[xi, yi - 1].setBlockType(Block.GRASS_BLOCK);
                }
                if (r < 1)
                {
                    Tree temp = new Tree(xi, yi);
                }
            }
        }
        #endregion
        #region "landgeneration"
        public void generateTerrain()
        {
            for (int x = 0; x < xD; x++)
            {
                for (int y = 0; y < yD; y++)
                {
                    terrain[x, y].setBlockType(Block.NO_BLOCK);
                }
            }
            GenerateMountains mountain1;
            mountain1 = new GenerateMountains(10, 200, 50, 10);
            //Random random = new Random();
            int Seed = (int)DateTime.Now.Ticks;
            Console.Out.WriteLine(Seed);
            random = new Random(Seed);
            float scaleOffset = (Seed / (Seed / 1.94875f)) / 2;
            float noiseScale = scale + scaleOffset;
            for (int xi = 1; xi < xD; xi++)
            {
                for (int yi = 1; yi < yD - 121; yi++)
                {
                    float offSet = ((float)yi / (float)yD);
                    if (Noise.GetNoise(xi / noiseScale, yi / noiseScale, Seed / 1000) < offSet * 1.5f)
                    {
                        terrain[xi, yi + 121].setBlockType(1);
                    }
                    else if (Noise.GetNoise(xi / noiseScale, yi / noiseScale, Seed / 1000) < offSet * 3)
                    {
                        terrain[xi, yi + 121].setBlockType(2);
                    }
                }
            }
            for (int xi = 1; xi < xD; xi++)
            {
                for (int yi = (int)(yD * .75f); yi < yD; yi++)
                {
                    if (random.Next(500) < 1)
                    {
                        terrain[xi, yi].setBlockType(Block.IRON_BLOCK);
                    }
                    if (random.Next(1000) < 1)
                    {
                        terrain[xi, yi].setBlockType(Block.GOLD_BLOCK);
                    }
                }
            }
            for (int xi = 2; xi < xD - 1; xi++)
            {
                for (int yi = 1; yi < yD - 1; yi++)
                {
                    checkVariation(xi, yi);
                }
            }
            for (int xi = 4; xi < xD - 3; xi++)
            {
                for (int yi = 4; yi < yD - 3; yi++)
                {
                    if (terrain[xi, yi].getBlockType() != Block.DIRT_BLOCK)
                    {
                        terrain[xi, yi].setVariation(0);
                    }
                    checkGrass(xi, yi);
                }
            }
        }
            #endregion
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace A3_NovelVisualization;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SpriteFont _font;
    private Color[] _palette;
    private List<string> _uniqueWords;
    private List<string> _shuffledWords;
    private List<string> _words;
    private List<Vector2> _positions;
    private List<Color> _cloudColors;

    private int _canvasWidth;
    private int _canvasHeight;

    private MouseState _previousMouse;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        _palette = new Color[]
        {
            Color.MediumPurple,
            Color.CornflowerBlue,
            Color.LightPink
        };
        
        _canvasWidth = 700;
        _canvasHeight = 600;
        
        _graphics.PreferredBackBufferWidth = _canvasWidth;
        _graphics.PreferredBackBufferHeight = _canvasHeight;
        _graphics.ApplyChanges();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        _font = Content.Load<SpriteFont>("fonts/font");

        String path = Path.GetFullPath("Content/uniquewords.txt");
        _uniqueWords = File.ReadAllLines(path).ToList();

        WordCloud();

    }

    private void WordCloud()
    {
        Random random = new Random();
        
        //new word cloud
        _words = new List<string>();
        _positions = new List<Vector2>();
        _cloudColors = new List<Color>();

        Vector2 current = new Vector2(20, 20);
        
        float lineHeight = _font.LineSpacing;
        float spacing = 10f;

        int c1 = random.Next(_palette.Length);
        _shuffledWords = _uniqueWords.OrderBy(x => random.Next()).ToList();

        int index = 0;

        foreach (var word in _shuffledWords)
        {
            Vector2 wordSize = _font.MeasureString(word);
            
            //wrapping
            if (current.X + wordSize.X > _canvasWidth)
            {
                current.X = 20;
                current.Y += lineHeight + spacing;
            }

            if (current.Y + wordSize.Y > _canvasHeight)
            {
                break;
            }
            
            _words.Add(word);
            _positions.Add(current);
            _cloudColors.Add(_palette[(c1 + index) % _palette.Length]);

            current.X += wordSize.X + spacing;
            index++;

        }
        
    }
    
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        MouseState mouse = Mouse.GetState();

        if (mouse.LeftButton == ButtonState.Pressed && _previousMouse.LeftButton == ButtonState.Released)
        {
            WordCloud();
        }

        _previousMouse = mouse;
        

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);
        
        // TODO: Add your drawing code here
        _spriteBatch.Begin();
        for (int i = 0; i < _words.Count; i++)
        {
            _spriteBatch.DrawString(
                _font,
                _words[i], 
                _positions[i],
                _cloudColors[i]
            );
        }
        
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}

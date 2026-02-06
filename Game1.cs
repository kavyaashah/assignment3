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
    private Color _color;
    private int _index;
    private List<string> _uniqueWords;
    private List<string> _displayedWords;
    private Vector2 _currentLineWidth;
    private Color[] _colors;

    private List<string> _words;
    private List<Vector2> _positions;
    private List<Color> _cloudColors;

    private int canvasWidth;
    private int canvasHeight;
    

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        _colors = new Color[]
        {
            Color.Lavender,
            Color.LightBlue,
            Color.LightPink
        };

        _currentLineWidth = new Vector2(0, 0);

        canvasWidth = 700;
        canvasHeight = 600;
        
        _graphics.PreferredBackBufferWidth = canvasWidth;
        _graphics.PreferredBackBufferHeight = canvasHeight;
        _graphics.ApplyChanges();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        _font = Content.Load<SpriteFont>("fonts/font");

        string path = Path.Combine(Content.RootDirectory, "uniquewords.txt");
        _uniqueWords = File.ReadAllLines(path).ToList();
        
    }

    private void WordCloud()
    {
        Random random = new Random();
        
        //new word cloud
        _words = new List<string>();
        _positions = new List<Vector2>();
        _colors = new List<Color>();

        Vector2 _current = new Vector2(0, 0);
        
        float lineHeight = _font.LineSpacing;

        int c1 = random.Next(3);
        List<string> shuffledWords = _uniqueWords.OrderBy(x => random.Next()).ToList();

        _index = 0;

        foreach (var word in shuffledWords)
        {
            Vector2 wordSize = _font.MeasureString(word);
            
            //wrapping
            if (_current.X + wordSize.X > canvasWidth)
            {
                _current.X = 20;
                _current.Y += lineHeight;
            }

            if (_current.Y + wordSize.Y > canvasHeight)
            {
                break;
            }
            
            _words.Add(word);
            _positions.Add(_current);
            _cloudColors.Add(_colors[(c1 + _index) % _colors.Length]);
            
        }



    }
    
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        _color = Color.White; 

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        
        // TODO: Add your drawing code here
        _spriteBatch.Begin();
        _spriteBatch.DrawString(
            _font,
            string, 
            Vector2.Zero,
            _color
        );
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}

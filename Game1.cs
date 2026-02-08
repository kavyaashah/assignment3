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
    
    //bools for display
    private bool display_unique;
    private bool _held;
    private KeyboardState _keyboardState;
    //variables for storing words counts
    private string wordfreq;
    private string[] lines;
    private string[] words;
    private Dictionary<string, int> wordCounts;
    private List<KeyValuePair<string, int>> sortedDict;
    //create our visualization
    private Texture2D chart;
    
    

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
        
        //create base for variables
        display_unique = true;
        wordCounts = new Dictionary<string, int>();
        chart = new Texture2D(_graphics.GraphicsDevice, 1, 1);
        chart.SetData<Color>(new Color[] { Color.White });//initialize texture color
        Window.TextInput += TextInputCallback;//take in text input
        
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
    private void TextInputCallback(object sender, TextInputEventArgs args)
    {
        //if enter key is pressed, switch displaying bools
        switch (args.Key)
        {
            case Keys.Enter:
                if (display_unique && !_held)
                {
                    display_unique = false;
                    _held = true;
                }
                else if ( !_held)
                {
                    display_unique = true;
                    _held = true;
                }

                break;
            
        }

    }
    
    private void BuildWordFrequency()
    {
        //to create a list of lines, break text block by newlines
        lines = wordfreq.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        
        foreach (string line in lines)
        {
            string[] parts = line.Split(':');// split lines on semicolon

            if (parts.Length < 2)
                continue;
            
            //first number in list is frequency, second number is count of frequency
            //.Trim() gets rid of whitespace
            string freq = parts[0].Trim();
            int num = int.Parse(parts[1].Trim());
            
            //create dictionary
            wordCounts[freq] = num;
        }
        //sort dictionary
        sortedDict = wordCounts.OrderByDescending(x => x.Value).ToList();
        
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        _font = Content.Load<SpriteFont>("fonts/font");

        String path = Path.GetFullPath("Content/uniquewords.txt");
        _uniqueWords = File.ReadAllLines(path).ToList();

        DisplayUniqueWords();
        
        using (var stream = TitleContainer.OpenStream("Content/files/wordTextFrequncy.txt"))
        using (var reader = new StreamReader(stream))
        {
            wordfreq = reader.ReadToEnd();
        }
        // build dictionary
        BuildWordFrequency();

    }
    

    private void DisplayUniqueWords()
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

    private void window_DisplayUniqueWords(SpriteBatch _spriteBatch)
    {
        for (int i = 0; i < _words.Count; i++)
        {
            _spriteBatch.DrawString(
                _font,
                _words[i], 
                _positions[i],
                _cloudColors[i]
            );
        }
    }

    private void DisplayWordFrequency(SpriteBatch _spriteBatch, Texture2D texture)
    {
        //if dict not created, exit
        if (sortedDict == null || sortedDict.Count == 0)
            return;
        //variables for display
        int totalBars = sortedDict.Count;
        //int s_width = Window.ClientBounds.Width;
        int s_height = Window.ClientBounds.Height;
        int spacing = 5;
        int barWidth = 10;
        //(s_width - 100) / totalBars
        int startx = 50;
        int basey = s_height - 50;
        int maxBarHeight = s_height - 150;
        int max = sortedDict[0].Value;

        //create rectangle for each key value pair in sortedDict
        for (int i = 0; i < totalBars; i++)
        {
            int count = sortedDict[i].Value;

            float normalizedHeight = (count / (float)(max))*maxBarHeight;

            Rectangle bar = new Rectangle(
                startx + i * (barWidth + spacing),
                basey - (int)normalizedHeight,
                barWidth,
                (int)normalizedHeight
            );
            //draw created rectangle to the texture (chart)
            _spriteBatch.Draw(texture, bar, Color.White);

        }
    }
    
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        /*
        MouseState mouse = Mouse.GetState();

        if (mouse.LeftButton == ButtonState.Pressed && _previousMouse.LeftButton == ButtonState.Released)
        {
            DisplayUniqueWords();
        }
        
        _previousMouse = mouse;
        */
        _keyboardState = Keyboard.GetState();
        if (!_keyboardState.IsKeyDown(Keys.Enter))
        {
            _held = false;
        }
        

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.LightSteelBlue);
        
        // TODO: Add your drawing code here
        _spriteBatch.Begin();
        /*
        for (int i = 0; i < _words.Count; i++)
        {
            _spriteBatch.DrawString(
                _font,
                _words[i], 
                _positions[i],
                _cloudColors[i]
            );
        }*/
        
        if (display_unique)
        {
            window_DisplayUniqueWords(_spriteBatch);
        }
        else
        {
            DisplayWordFrequency(_spriteBatch, chart);
        }
        
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}






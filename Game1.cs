using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;

namespace private_game3;


public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
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
        chart.SetData(new[] { Color.White });//initialize texture color
        Window.TextInput += TextInputCallback;//take in text input

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
    
    
    //This function 
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
    
    //creates the actual visualization
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

    private void DisplayUniqueWords()
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
        }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        //read in wordTextFrequency file
        using (var stream = TitleContainer.OpenStream("Content/files/wordTextFrequncy.txt"))
        using (var reader = new StreamReader(stream))
        {
            wordfreq = reader.ReadToEnd();
        }
        // build dictionary
        BuildWordFrequency();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        //make sure the enter button is not being held
        _keyboardState = Keyboard.GetState();
        if (!_keyboardState.IsKeyDown(Keys.Enter))
        {
            _held = false;
        }

        // TODO: Add your update logic here
        

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.LightSteelBlue);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();
        //if statement for displaying word cloud or word frequencies
        if (display_unique)
        {
            DisplayUniqueWords();
        }
        else
        {
            DisplayWordFrequency(_spriteBatch, chart);
        }
        base.Draw(gameTime);
        _spriteBatch.End();
    }
}
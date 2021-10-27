using System;
using static System.Console;

using System.Threading;
using System.Threading.Tasks;

using System.Collections;
using System.Collections.Generic;

public class ConsoleInterface {

    public Game game;

    public bool run_console = false;

    public int window_height = 0;
    public int window_width = 0;

    private Thread MainThread;

    public ConsoleInput input;
    private WriteParser parser;

    private List<format_string> main_buffer;
    private bool buffer_updated = false;

    private struct format_string {
        public string content;
        public FormatOptions format;
    }

    private List<ConsoleBox> boxes;
    private enum BoxConfig {
        Default,
        Stylised,
        Custom,

    }

    public enum FormatOptions {
        LeftCenter,
        MiddleCenter,
        RightCenter,
    }

    private BoxConfig box_options;
    private ConsoleColor box_bg = ConsoleColor.Black;
    private ConsoleColor box_fg = ConsoleColor.White;

    public ConsoleInterface(Game g) {
        game = g;
    }

    public void init() {

        input = new ConsoleInput();
        parser = new WriteParser();
        

        MainThread = new Thread(UpdateConsole);
        MainThread.Name = "console_thread";
        MainThread.Start();

        main_buffer = new List<format_string>();
        boxes = new List<ConsoleBox>();

        this.run_console = true;
        input.init();
        
    }

    public void clearScr() {
        main_buffer.Clear();
    }

    public void writeScr(string to_write) {
        if (string.IsNullOrEmpty(to_write)) {
            return;
        }
        main_buffer.Add(new format_string {
            content = to_write,
            format = FormatOptions.LeftCenter
        });
        buffer_updated = true;
    }

    public void writeF(string to_write, FormatOptions fo) {
        if (string.IsNullOrEmpty(to_write)) {
            return;
        }
        main_buffer.Add(new format_string {
            content = to_write,
            format = fo
        });
        buffer_updated = true;
    }

    public void warnScr(string warning) {
        if (string.IsNullOrEmpty(warning)) {
            return;
        }
        main_buffer.Add(new format_string {
            content = "|red|" + warning,
            format = FormatOptions.RightCenter
        });
        buffer_updated = true;
    }

    public ConsoleBox newBox(int x, int y, int w, int h) {
        ConsoleBox boc = new ConsoleBox(x, y, w, h);
        boxes.Add(boc);
        return boc;
    }

    private int default_cursorX = 0;

    public void UpdateConsole() {

        if (checkResize() == true) {
            drawOutline();
            write_buffer();
            int final_x = 2, final_y = 3;
            foreach (ConsoleBox box in boxes) {
                box.drawOutline(window_height, window_width);
                if (box.input_box) {
                    final_x = box.originx + 2;
                    default_cursorX = box.originx + 2;
                    final_y = box.height - 3;
                }
            }
            SetCursorPosition(final_x, final_y);
        }
        
        game.player.sendInput(input.input);
        if (input.input != null) {
            redrawBoxes();
            write_buffer();
            int final_x = 2, final_y = 3;
            foreach (ConsoleBox box in boxes) {
                box.drawOutline(window_height, window_width);
                if (box.input_box) {
                    final_x = box.originx + 2;
                    default_cursorX = box.originx + 2;
                    final_y = box.height - 3;
                }
            }
            SetCursorPosition(final_x, CursorTop);
        }
        input.input = null;

        if (buffer_updated == true) {
            redrawBoxes();
            write_buffer();
            int final_x = 2, final_y = 3;
            if (boxes.Count > 0) {
                foreach (ConsoleBox box in boxes) {
                    box.drawOutline(window_height, window_width);
                    box.write_buffer(window_width, parser);
                    if (box.input_box) {
                        final_x = box.originx + 2;
                        default_cursorX = box.originx + 2;
                        final_y = box.height - 3;
                    }
                }
                
            }
            SetCursorPosition(final_x, final_y);
        }

        

        Thread.Sleep(100);
        if (run_console == true) {
            UpdateConsole();
        }
        else {
            input.run_input = false;
        }

    }   

    public void clrScr() {
        for (int i = 1; i < window_width; i ++) {
            for (int b = 1; b < window_height; b ++) {
                SetCursorPosition(i, b);
                Write(" ");
            }
        }
    }

    public void redrawBoxes() {
        drawOutline();
        int final_x = 2, final_y = 3;
            foreach (ConsoleBox box in boxes) {
                box.drawOutline(window_height, window_width);
                if (box.input_box) {
                    final_x = box.originx + 2;
                    final_y = box.height - 3;
                }
            }
            SetCursorPosition(final_x, final_y);
    }

    private void write_buffer() {

        int substr_index = 0; // to prevent the formatting from ignoring som esubstrings when it counts the total
            // (it counts everything even substrings weve already discarded)

        SetCursorPosition(2, 1);
        foreach (format_string str in main_buffer)  { // get all string in the buffer

            substr_index = 0;
            
            foreach(WriteParser.SubString g in parser.parseString(str.content)) { // get all the substrings
                
                

                string total = "";

                foreach (WriteParser.SubString temp in parser.parseString(str.content)) {
                    total += temp.content;
                }

                //Console.WriteLine("tote: "+ total);
                
                //ReadLine();

                if (substr_index == 0) { 
                    
                    switch (str.format) {
                        case FormatOptions.LeftCenter:
                            break;

                        case FormatOptions.MiddleCenter:
                            if (window_width < total.Length) {
                                break;
                            }
                            SetCursorPosition((window_width - total.Length) / 2, CursorTop);
                            break;

                        case FormatOptions.RightCenter:
                            if (window_width < total.Length) {
                                break;
                            }
                            SetCursorPosition((window_width - 2) - total.Length, CursorTop);
                            break;

                        default:
                            break;
                    }
                    //WriteLine("substring 0");
                    //ReadLine();
                }

                ForegroundColor = g.fg_colour; //set the fg and bg colour for each substring
                BackgroundColor = g.bg_colour;
                int index = 0;
                foreach (char ch in g.content) { // go through each char checking for a newline or the end of the screen
                    if (ch == '\n') {
                        total = total.Substring(index + 1);
                            switch (str.format) {
                            case FormatOptions.LeftCenter:
                                break;

                            case FormatOptions.MiddleCenter:
                                if (window_width < total.Length) {
                                    break;
                                }
                                SetCursorPosition((window_width - total.Length) / 2, CursorTop + 1);
                                break;

                            case FormatOptions.RightCenter:
                                if (window_width < total.Length) {
                                    break;
                                }
                                SetCursorPosition((window_width - 2) - total.Length, CursorTop + 1);
                                break;

                            default:
                                SetCursorPosition(3, CursorTop + 1);
                                break;
                        }
                        continue;
                    }
                    Write(ch);
                    if (CursorLeft > window_width - 2) {
                        total = total.Substring(index + 1);
                            switch (str.format) {
                            case FormatOptions.LeftCenter:
                                break;

                            case FormatOptions.MiddleCenter:
                                if (window_width < total.Length) {
                                    break;
                                }
                                SetCursorPosition((window_width - total.Length) / 2, CursorTop + 1);
                                break;

                            case FormatOptions.RightCenter:
                                if (window_width < total.Length) {
                                    break;
                                }
                                SetCursorPosition((window_width - 2) - total.Length, CursorTop + 1);
                                break;

                            default:
                                SetCursorPosition(3, CursorTop + 1);
                                break;
                        }
                    }
                    index ++;
                }

                substr_index ++;

            }
            SetCursorPosition(2, CursorTop + 1);
        }

        ForegroundColor = box_fg;
        BackgroundColor = box_bg;

        buffer_updated = false;

    }

    // function to draw a box around the console window
    // changes in function of the 'box_options' enum
    private void drawOutline() {
        Clear();
        ForegroundColor = box_fg;
        BackgroundColor = box_bg;
        switch(box_options) {
            case BoxConfig.Default:
                char seperator = '█';
                SetCursorPosition(0, 0);
                // ceilings
                for (int i = 0; i < window_width; i ++) {
                    Write(seperator);
                }
                SetCursorPosition(0, window_height - 1);
                for (int i = 0; i < window_width; i ++) {
                    Write(seperator);
                }
                // walls
                SetCursorPosition(0, 0);
                for (int i = 0; i < window_height; i ++) {
                    Write(seperator);
                    SetCursorPosition(0, i);
                }
                SetCursorPosition(window_width - 2, 0);
                for (int i = 0; i < window_height; i ++) {
                    Write(seperator);
                    SetCursorPosition(window_width - 1, i);
                }
                break;

            default:
                goto case BoxConfig.Default;
        }
        
    }

    // function to check if the window has resized and if it has reset the appropriate height and 
    // width values
    private bool checkResize() {
        if (window_height != Console.WindowHeight || window_width != Console.WindowWidth) {
            window_height = Console.WindowHeight;
            window_width = Console.WindowWidth;
            return true;
        }
        return false;
    }

}

public class ConsoleBox {

    private List<string> main_buffer;
    private bool buffer_updated = false;
    private char seperator = '█';

    public ConsoleColor box_fg = ConsoleColor.White;
    public ConsoleColor box_bg = ConsoleColor.Black;

    public bool input_box;

    public  int originx, originy;
    public int width, height;

    public ConsoleBox(int x, int y, int xx, int yy) {
        main_buffer = new List<string>();
        input_box = false;
        originx = x;
        originy = y;
        width = xx;
        height = yy;
    }

    public void clrScr() {
        for (int i = 1; i < this.width; i ++) {
            for (int b = 1; b < this.height; b ++) {
                SetCursorPosition(i, b);
                Write("");
            }
        }
    }

    public void writeScr(string to_write) {
        if (string.IsNullOrEmpty(to_write)) {
            return;
        }
        main_buffer.Add(to_write);
        buffer_updated = true;
    }

    public void write_buffer(int window_width, WriteParser parser) {

        SetCursorPosition(originx + 2,  originy + 1);
        foreach (string str in main_buffer)  {
            //Console.WriteLine(str);
            foreach(WriteParser.SubString g in parser.parseString(str)) {
                //Console.WriteLine("g content: " + g.content); // im getting the colour as the content
                ForegroundColor = g.fg_colour;
                BackgroundColor = g.bg_colour;
                foreach (char ch in g.content) {
                    if (ch == '\n') {
                        SetCursorPosition(originx + 2, CursorTop + 1);
                        continue;
                    }
                    Write(ch);
                    if (CursorLeft > width - 2) {
                        SetCursorPosition(originx + 2, CursorTop + 1);
                    }
                }
            }
            SetCursorPosition(originx + 2, CursorTop + 1);
        }

        buffer_updated = false;

    }

    public void drawOutline(int window_height, int window_width) {

        
        if (height > window_height ) height = window_height - 1;
        if (width > window_width ) width = window_width - 1;


        ForegroundColor = box_fg;
        BackgroundColor = box_bg;
        
            SetCursorPosition(originx, originy);
            // ceilings
            for (int i = originx; i < width; i ++) {
                Write(seperator);
            }
            SetCursorPosition(originx, height - 1);
            for (int i = originx; i < width; i ++) {
               Write(seperator);
            }
            // walls
            SetCursorPosition(originx, originy);
            for (int i = originy; i < height; i ++) {
                Write(seperator);
                SetCursorPosition(originx, i);
            }
            SetCursorPosition(width, originy);
            for (int i = originy; i < height; i ++) {
                Write(seperator);
                SetCursorPosition(width, i);
            }

           
    }

    

}


public class ConsoleInput {

    public bool new_input = false;

    public bool run_input = false;

    private Thread InputThread;

    public string input;

    

    public ConsoleInput() {

    }

    public void init() {

        input = "";
        
        InputThread = new Thread(AwaitInput);
        InputThread.Name = "input_thread";
        InputThread.Start();

        run_input = true;
    }

    public void AwaitInput() {
        while (run_input)
        {
            if(new_input) {
                input = System.Console.ReadLine();
                new_input = false;
            }
        }
    }


}
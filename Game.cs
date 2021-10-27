

public class Game {

    public ConsoleInterface console;

    public PlayerInterface player;
    public DataInterface data;
    public World world;

    public bool running = false;

    public Game() {
        console = new ConsoleInterface(this);
        player = new PlayerInterface(this);
        world = new World();
        data = new DataInterface(world);
        console.init();
        running = true;
        this.run();
    }

    public void end() {
        console.run_console = false;
    }

    private void run() {

        data.getFirstRoom();
        data.loadItems();

        // good so far, bug where if you resize the console during a tick it crashes and i cant even begin to think of
        // how to solve it since its a thing with the console and not me
        // also might wanna try and make the boxes resize with the screen :raised_eyebrows:
        // anyway now u just needa think of a game 

        /*ConsoleBox input = console.newBox(0, 50, 1000, 1000);
        input.input_box = true;*/


        /*console.writeScr("hi  |cyan|rr|villa|r"); // works 
        console.writeScr("aaa");
        console.writeF("|white||white|i|red|m in the midd|blue|le!!! and blue", ConsoleInterface.FormatOptions.MiddleCenter);
        ConsoleBox bx = console.newBox(10, 20, 20, 30);
        bx.writeScr("poopp|green|ppppppppppp|red|pee");
        ConsoleBox bx2 = console.newBox(100, 10, 200, 200);
        bx.input_box = true;
        console.writeF("hihi i am on the right uwu |gr/r|gr?", ConsoleInterface.FormatOptions.RightCenter);*/

    }



}
using static World;

using System;
public class PlayerInterface {

    private Game game;

    private bool quit = false;

    public PlayerInterface(Game g) {
        game = g;
    }

    public void sendInput(string input) {
        game.console.input.new_input = false;
        parseInput(input);
        if (!quit) game.console.input.new_input = true;
    }

    private bool level_editing_mode = false;

    private void parseInput(string input) {

        if (string.IsNullOrEmpty(input)) {
            return;
        }

        if (input[0] == ']') {
            editor_commands(input.Split(']',2)[1]);
        }

        if (level_editing_mode == false) {
            default_input(input);
        } else {
            level_editor(input);
        }
    }


    private void editor_commands(string input) {

        game.console.warnScr(input);

        string[] input_split = input.Split(' ');

        switch(input_split[0]) {

            case "room":
                game.console.writeScr("name -> [|red|" + game.world.world_rooms[0].name + "|fg|]");
                game.console.writeScr("[|red|tag|fg|] - " + game.world.world_rooms[0].tag);
                game.console.writeScr("[|red|desc|fg|] - " + game.world.world_rooms[0].description);
                game.console.writeScr("[|red|---ITEMS---|fg|]");
                game.console.writeScr("[|red|" + game.world.world_rooms[0].get_items_as_string() + "|fg|]");
                break;

            default:
                game.console.writeScr("|red|Warn|fg| -> unknown editor command.");
                break;

        }

    }

    private Room current_room = new Room();

    private void level_editor(string input) {

        int index = 0;

        Console.Clear();
        game.console.clearScr();
        game.console.redrawBoxes();

        if (current_room == null) {
            game.console.writeF("[|red|WARNING|fg|] - there is no current room, the first room found will be used, or a new one will be created.", ConsoleInterface.FormatOptions.MiddleCenter);
            if (game.world.world_rooms != null) {
                if (game.world.world_rooms.Count != 0) {
                    game.console.writeScr("Found a room, using |grey|world_rooms[0]|fg|.");
                    current_room = game.world.world_rooms[0];
                }
            } else {
                game.console.writeScr("No rooms were found, creating a new one.");
                current_room = new Room();
                current_room.name = "new";
                current_room.description = "new description";
                current_room.tag = "default_tag";
            }
        }

        game.console.writeF("name -> [|red|" + current_room.name + "|fg|] <- name", ConsoleInterface.FormatOptions.MiddleCenter);
        game.console.writeScr("[|red|tag|fg|] - " + current_room.tag);
        game.console.writeScr("[|red|desc|fg|] - " + current_room.description);

        string[] inp = input.Substring(0).Split(' ');

        switch(inp[0]) {

            case "help":
                game.console.writeScr("[|red|setname|fg|] - sets the name of the current room");
                game.console.writeScr("[|red|setdesc|fg|] - sets the description of the current room");
                game.console.writeScr("[|red|settag|fg|] - sets the tag of the current room");
                game.console.writeScr("[|red|------|fg|]");
                game.console.writeScr("[|red|listitems|fg|] - lists the items of the current room");
                game.console.writeScr("[|red|listexits|fg|] - lists the exits of the current room");
                game.console.writeScr("[|red|listobjs|fg|] - lists the interactables of the current room");
                game.console.writeScr("[|red|------|fg|]");
                game.console.writeScr("[|red|additem|fg|] - adds an item to the current room, usage: additem [name] [tag] [desc] [aliases]");
                game.console.writeScr("[|red|exit|fg|] - exits the level editor");
                break;

            case "setname":
                if (inp.Length < 2) {
                    too_few_args_warn("setname");
                    return;
                }
                current_room.name = input.Split(' ', 2)[1];
                break;
            
            case "setdesc":
                if (inp.Length < 2) {
                    too_few_args_warn("setdesc");
                    return;
                }
                current_room.description = input.Split(' ', 2)[1];
                break;

            case "settag":
                if (inp.Length < 2) {
                    too_few_args_warn("settag");
                }
                current_room.tag = input.Split(' ', 2)[1];
                break;

            case "listitems":
                index = 0;
                foreach (Item it in current_room.items) {
                    game.console.writeScr(index + ": " + it.name);
                    index ++;
                }
                break;

            case "listexits":
                index = 0;
                foreach (Passage it in current_room.exits) {
                    game.console.writeScr(index + ": " + it.tag);
                    index ++;
                }
                break;

            case "listobjs":
                index = 0;
                foreach (Interactable it in current_room.interactables) {
                    game.console.writeScr(index + ": " + it.name);
                    index ++;
                }
                break;

            case "additem":
                if (inp.Length < 5) {
                    too_few_args_warn("tellitem");
                    return;
                }
                current_room.items.Add(new Item {
                    name = inp[1],
                    tag = inp[2],
                    description = inp[3],
                    aliases = inp[4].Split('/')
                });
                break;

            case "tellitem":
                if (inp.Length < 2) {
                    too_few_args_warn("tellitem");
                    return;
                }
                int i = 0;
                try {
                    i = int.Parse(inp[1]);
                } catch {
                    non_numerical_value("tellitem");
                    return;
                }
                if (i > current_room.items.Count) {
                    out_of_range_value("tellitem");
                    return;
                }
                Item tempo = current_room.items[i];
                game.console.writeScr("[|red|name|fg|] - " + tempo.name);
                game.console.writeScr("[|red|tag|fg|] - " + tempo.tag);
                game.console.writeScr("[|red|desc|fg|] - " + tempo.description);
                game.console.writeScr("[|red|aliases|fg|] - " + String.Join('/', tempo.aliases));
            break;

            default:
                break;

        }

    }

    private void out_of_range_value(string str) {
        game.console.writeScr("[|red|warning|fg|] - Out of range value(s) for command [|cyan|" + str + "|fg|]");
    }

    private void non_numerical_value(string str) {
        game.console.writeScr("[|red|warning|fg|] - Non numerical argument(s) for command [|cyan|" + str + "|fg|]");
    }

    private void too_few_args_warn(string str) {
        game.console.writeScr("[|red|warning|fg|] - Too few arguments for command [|cyan|" + str + "|fg|]");
    }



    private void default_input(string input) {
        switch(input) {
            case "exit":
            case "quit":
                quit = true;
                game.end();
                break;

            case "cls":
                game.console.clearScr();
                break;

            default:
                break;
        }
    }

    /*private void levelEditor() {

        game.console.input.run_input = false;

        string input = System.Console.ReadLine();

        while(input != "]quit") {

            game.console.writeScr("|red|LEVEL EDITOR|fg|");

            int index = 0;
            foreach (Room r in game.world.world_rooms) {
                game.console.writeScr(index + "[" + r.name + "] ");
                index ++;
            }

            




            input = Console.ReadLine();

        }

        game.console.input.run_input = true;

    }*/



}
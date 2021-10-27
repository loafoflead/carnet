using System.Collections.Generic;
using System.Collections;

public class World {


    public World() {
        world_rooms = new List<Room>();
        world_items = new List<Item>();
        world_interactables = new List<Interactable>();
    }


    public List<Room> world_rooms;
    public Room current_room;

    public List<Item> world_items;
    public List<Interactable> world_interactables;



    public List<Item> getItemsFromTags(string tags) {
        List<Item> to_return = new List<Item>();
        if (!tags.Contains('/')) {
            if (world_items.Find(Item => Item.name == tags) != null) {
                to_return.Add(world_items.Find(Item => Item.tag == tags));
            } 
        }
        else {

            foreach (string str in tags.Split('/')) {
                if (world_items.Find(Item => Item.tag == str) != null) {
                    to_return.Add(world_items.Find(Item => Item.tag == tags));
                } 
            }

        }
        return to_return;
    }


    public void addRoom(
        string n, string desc,
        List<Item> it, List<Interactable> itr, List<Passage> pass
    ) 
    {
        world_rooms.Add( new Room {
            name = n,
            description = desc,
            items = it,
            interactables = itr,
            exits = pass,
        });
    }

    public class Room {
        public string tag;
        public string name;
        public string description;
        public List<Item> items;
        public List<Passage> exits;
        public List<Interactable> interactables;

        public Room() {
            this.items = new List<Item>();
            this.exits = new List<Passage>();
            this.interactables = new List<Interactable>();
        }

        public string get_items_as_string() {
            string return_string = "";
            foreach(Item it in this.items) {
                return_string += it + "/";
            }
            if(return_string.Length > 2) return_string.Remove(return_string.Length - 1);
            return return_string;
        }

    }

    public class Passage {
        public string tag;
        public string[] aliases;
        public string travel_dialogue;
        public Room room_from;
        public Room room_to;
        public bool locked;
        public string locked_dialogue;
        public string passthrough_action;
    }

    public class Item {
        public string tag;
        public string name;
        public string description;
        private string pr_aliases;
        public string[] aliases {
            get {
                if (!pr_aliases.Contains('/')) {
                    string[] n = new string[1];
                    n[0] = pr_aliases;
                    return n;
                } else {
                    return pr_aliases.Split('/');
                }
            }
        
            set {
                pr_aliases = System.String.Join('/', value);
            }
        }
        public void set_aliases_str(string str) {
            pr_aliases = str;
        }

        public string tell_item() {
            string to_ret = "";
            to_ret += this.tag + "/";
            to_ret += this.name + "/";
            to_ret += this.description + "/";
            to_ret += this.pr_aliases;
            return to_ret;
        }
    }

    public class Interactable {
        public string name;
        public string tag; 
        public string[] aliases;
        public string description;
        public string used_action;
    }


}
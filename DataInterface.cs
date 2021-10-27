using static World;

using System;
using System.Data.SQLite;

using System.Collections;
using System.Collections.Generic;

public class DataInterface {

    string cs = @"URI=file:C:\Users\benja\Documents\carnet\file.sqli";

    SQLiteConnection con;
    SQLiteCommand cmd; 
    SQLiteDataReader rdr;

    private World world;


    public void getFirstRoom() {

        Room rm = new Room();

        string stm = "SELECT * FROM rooms LIMIT 5";

        cmd = new SQLiteCommand(stm, con);

        rdr = cmd.ExecuteReader();
        //Console.WriteLine($"{rdr.GetName(0), -3} {rdr.GetName(1), -8} {rdr.GetName(2), 8} {rdr.GetName(3), 13} {rdr.GetName(4), 20} {rdr.GetName(5), 26} {rdr.GetName(6), 30}");

        while (rdr.Read()) {
            rm.tag = rdr.GetString(1).Replace("\n", "").Replace("\t", "    ");
            rm.name = rdr.GetString(2).Replace("\n", "").Replace("\t", "    ");
            rm.description = rdr.GetString(3).Replace("\n", "").Replace("\t", "    ");
            rm.items = world.getItemsFromTags(rdr.GetString(4).Replace("\n", "").Replace("\t", "    "));
            world.world_rooms.Add(rm);
        }

        

    }

    public void loadItems() {
        string stm = "SELECT * FROM rooms LIMIT 5";

        cmd = new SQLiteCommand(stm, con);

        rdr = cmd.ExecuteReader();

        Item current = new Item();

        while(rdr.Read()) {
            current.tag = rdr.GetString(1);
            current.name = rdr.GetString(2);
            current.description = rdr.GetString(3);
            current.set_aliases_str(rdr.GetString(4));
            Console.WriteLine(current.tell_item());
            world.world_items.Add(current);
        }

        Console.WriteLine(current.tell_item());

    }

    public DataInterface(World w) {

        world = w;

        con = new SQLiteConnection(cs);
        con.Open();
        cmd = new SQLiteCommand(con);
        //rdr = cmd.ExecuteReader();
        /*cmd.CommandText = @"DROP TABLE IF EXISTS rooms";
        cmd.ExecuteNonQuery();
        cmd.CommandText = @"CREATE TABLE rooms(id INTEGER PRIMARY KEY, name TEXT)";
        cmd.ExecuteNonQuery();*/
    }

    public void add_data(string data) {
        cmd.CommandText = "INSERT INTO rooms(name) VALUES('" + data + "')";
        cmd.ExecuteNonQuery();
    }

    public void getWorldData() {

    }

}
using System.Numerics;
using DarkRift;
public class NetworkingData
{
    public enum Tags
    {
        LoginRequest = 0,
        LoginRequestAccepted = 1,
        LoginRequestDenied = 2,
        PlayerReady = 3,
        GameStartData = 100,
        GameUpdate = 200,
        GamePlayerInput = 203
    }

    
    public struct LoginRequestData : IDarkRiftSerializable
    {
        public string Name;

        public LoginRequestData(string name)
        {
            Name = name;
        }

        public void Deserialize(DeserializeEvent e)
        {
            Name = e.Reader.ReadString();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Name);
        }
    }
    
    public struct LoginInfoData : IDarkRiftSerializable
    {
        public ushort Id;

        public LoginInfoData(ushort id)
        {
            Id = id;
        }

        public void Deserialize(DeserializeEvent e)
        {
            Id = e.Reader.ReadUInt16();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Id);
        }
    }

    public struct PlayerReadyData : IDarkRiftSerializable
    {
        public bool Ready;

        public PlayerReadyData(bool ready)
        {
            Ready = ready;
        }
        
        public void Deserialize(DeserializeEvent e)
        {
            Ready = e.Reader.ReadBoolean();
        }
 
        public void Serialize(SerializeEvent e)
        {
 
            e.Writer.Write(Ready);
        }
    }
    
    public struct PlayerInputData : IDarkRiftSerializable
    {
        public bool[] Keyinputs; // 0 = w, 1 = a, 2 = s, 3 = d, 4 = space, 5 = leftClick
        public byte LookDirection;
        public uint Time;
 
        public PlayerInputData(bool[] keyInputs, byte lookDirection, uint time)
        {
            Keyinputs = keyInputs;
            LookDirection = lookDirection;
            Time = time;
        }
 
        public void Deserialize(DeserializeEvent e)
        {
            Keyinputs = e.Reader.ReadBooleans();
            LookDirection = e.Reader.ReadByte();
 
            if (Keyinputs[5])
            {
                Time = e.Reader.ReadUInt32();
            }
        }
 
        public void Serialize(SerializeEvent e)
        {
 
            e.Writer.Write(Keyinputs);
            e.Writer.Write(LookDirection);
 
            if (Keyinputs[5])
            {
                e.Writer.Write(Time);
            }
        }
    }
    
    public struct PlayerStateData : IDarkRiftSerializable
    {
        public ushort Id;
        public Vector2 Position;
        public byte LookDirection;

        public PlayerStateData(ushort id, Vector2 position, byte lookDirection)
        {
            Id = id;
            Position = position;
            LookDirection = lookDirection;
        }

        public void Deserialize(DeserializeEvent e)
        {
            Position = new Vector2(e.Reader.ReadSingle(), e.Reader.ReadSingle());
            LookDirection = e.Reader.ReadByte();
            Id = e.Reader.ReadUInt16();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Position.X);
            e.Writer.Write(Position.Y);
            e.Writer.Write(LookDirection);
            e.Writer.Write(Id);
        }
    }
    
    public struct PlayerSpawnData : IDarkRiftSerializable
    {
        public ushort Id;
        public string Name;
        public Vector2 Position;

        public PlayerSpawnData(ushort id, string name, Vector2 position)
        {
            Id = id;
            Name = name;
            Position = position;
        }

        public void Deserialize(DeserializeEvent e)
        {
            Id = e.Reader.ReadUInt16();
            Name = e.Reader.ReadString();
            Position = new Vector2(e.Reader.ReadSingle(), e.Reader.ReadSingle());
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Id);
            e.Writer.Write(Name);
            e.Writer.Write(Position.X);
            e.Writer.Write(Position.Y);
        }
    }

    public struct PlayerDespawnData : IDarkRiftSerializable
    {
        public ushort Id;

        public PlayerDespawnData(ushort id)
        {
            Id = id;
        }

        public void Deserialize(DeserializeEvent e)
        {
            Id = e.Reader.ReadUInt16();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Id);
        }
    }
    
    public struct GameUpdateData : IDarkRiftSerializable
    {
        public uint Frame;
        public PlayerSpawnData[] SpawnData;
        public PlayerDespawnData[] DespawnData;
        public PlayerStateData[] UpdateData;

        public GameUpdateData(uint frame, PlayerStateData[] updateData, PlayerSpawnData[] spawnData, PlayerDespawnData[] despawnData)
        {
            Frame = frame;
            UpdateData = updateData;
            DespawnData = despawnData;
            SpawnData = spawnData;
        }

        public void Deserialize(DeserializeEvent e)
        {
            Frame = e.Reader.ReadUInt32();
            SpawnData = e.Reader.ReadSerializables<PlayerSpawnData>();
            DespawnData = e.Reader.ReadSerializables<PlayerDespawnData>();
            UpdateData = e.Reader.ReadSerializables<PlayerStateData>();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Frame);
            e.Writer.Write(SpawnData);
            e.Writer.Write(DespawnData);
            e.Writer.Write(UpdateData);
        }
    }
    
    public struct GameStartData : IDarkRiftSerializable
    {
        public uint OnJoinServerTick;
        public PlayerSpawnData[] Players;

        public GameStartData(PlayerSpawnData[] players, uint serverTick)
        {
            Players = players;
            OnJoinServerTick = serverTick;
        }

        public void Deserialize(DeserializeEvent e)
        {
            OnJoinServerTick = e.Reader.ReadUInt32();
            Players = e.Reader.ReadSerializables<PlayerSpawnData>();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(OnJoinServerTick);
            e.Writer.Write(Players);
        }
    }
}
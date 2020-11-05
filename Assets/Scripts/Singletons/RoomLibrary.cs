using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ROOM_NUMBER
{
    ZERO_ZERO_ZERO,
    ZERO_ZERO_ONE,
    ZERO_ZERO_TWO,
    Room_003, Room_004,
    Room_005, Room_006,
    Room_007, Room_008,
    END
}
public class RoomLibrary : Singleton<RoomLibrary>
{
    [SerializeField]
    private List<Room> mRooms;

    private Dictionary<ROOM_NUMBER, Room> mLibrary;

    private void Awake()
    {
        mLibrary = new Dictionary<ROOM_NUMBER, Room>();

        foreach (Room iterator in mRooms)
        {
            if (!mLibrary.ContainsKey(iterator.RoomNumber))
            {
                mLibrary.Add(iterator.RoomNumber, iterator);
            }
        }
    }

    #region READ
    /// <summary>
    /// RoomLibrary에 등재된 무작위 방을 반환합니다. 반환값은 중복될 수 있습니다.
    /// </summary>
    #endregion
    public Room Random()
    {
        int randomIndex = UnityEngine.Random.Range(0, mRooms.Count);

        return mRooms[randomIndex];
    }
}

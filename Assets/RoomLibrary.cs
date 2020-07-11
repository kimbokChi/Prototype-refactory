using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLibrary : Singleton<RoomLibrary>
{
    private Dictionary<ROOM_NUMBER, Room> mLibrary = new Dictionary<ROOM_NUMBER, Room>();

    [SerializeField]
    private List<Room> mRooms = new List<Room>();

    private System.Random mRandom = new System.Random();

    private void Awake()
    {
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
        int RandomIndex = mRandom.Next(0, (int)ROOM_NUMBER.END);

        if (mLibrary.ContainsKey((ROOM_NUMBER)RandomIndex))
        {
            return mLibrary[(ROOM_NUMBER)RandomIndex];
        }
        return null;
    }
}

public enum DIRECTION9
{
    TOP_LEFT, TOP, TOP_RIGHT,

    MID_LEFT, MID, MID_RIGHT,
    
    BOT_LEFT, BOT, BOT_RIGHT,
    
    END
}
public enum LPOSITION3
{
    TOP, MID, BOT, NONE
}

public enum TPOSITION3
{
    LEFT, MID, RIGHT, NONE
}

public enum BUFF
{
    HEAL, SPEEDUP, POWER_BOOST
}

/* README
 * MOVE [이동]
 * 
 * ATTACK [타격]
 * 
 * DAMAGE [피격]
 * 
 * ENTER [입장]
 * 
 * CHARGE [충전]
 */
public enum ITEM_KEYWORD
{
    MOVE_BEGIN, MOVE_END, ATTACK, BE_DAMAGED, ENTER
}

public enum ITEM_RATING
{
    COMMON, RARE, EPIC, LEGENDARY
}

public enum ITEM_DATA
{
    GOLDEN_FLIP,
    SLIVER_FLIP,
    BRONZE_FLIP,
    IRON_FLIP
}

public enum SLOT_TYPE
{
    CONTAINER, ACCESSORY, WEAPON
}

public enum MESSAGE
{
    THIS_ROOM, BELONG_FLOOR
}

public enum ROOM_NUMBER
{
    ZERO_ZERO_ZERO,
    ZERO_ZERO_ONE,
    ZERO_ZERO_TWO,
    END
}
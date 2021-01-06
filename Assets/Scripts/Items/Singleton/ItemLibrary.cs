using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ItemRating
{
    Common, Rare, Epic, Legendary
}
public class ItemLibrary : Singleton<ItemLibrary>
{
    public event System.Action<Item> ItemUnlockEvent;

    [Header("Item Probablities")]
    [SerializeField][Range(0f, 1f)] private float Common;
    [SerializeField][Range(0f, 1f)] private float Rare;
    [SerializeField][Range(0f, 1f)] private float Epic;
    [SerializeField][Range(0f, 1f)] private float Legendary;

    [Header("Registered Items")]
    [SerializeField] private RegisteredItem RegisteredItem;

    private Dictionary<ItemRating, List<Item>> mLibrary;
    private List<Item>   _LockedItemListForTest;
    private List<Item> _UnlockedItemListForTest;

    private float[] mProbabilityArray;

    [ContextMenu("Probablity Check")]
    private void ProbablityCheck()
    {
        float sum = Common + Rare + Epic + Legendary;

        if (sum == 1)
        {
            Debug.Log($"Probablity Sum : {sum}");
        }
        else
            Debug.LogError($"Probablity Sum : {sum}");
    }

    private void Awake()
    {
        LoadItemListForTest();

        SceneManager.sceneUnloaded += scene =>
        {
            ItemStateSaver.Instance.SaveLibDictionary(mLibrary);

            ItemStateSaver.Instance.SaveUnlockedItemListForTest(_UnlockedItemListForTest);
            ItemStateSaver.Instance.SaveLockedItemListForTest(_LockedItemListForTest);
        };
        mProbabilityArray = new float[4]
        {
            Common, Rare, Epic, Legendary
        };
        if (!ItemStateSaver.Instance.LoadLibDictionary(out mLibrary))
        {
            mLibrary.Add(ItemRating.Common,    new List<Item>());
            mLibrary.Add(ItemRating.Rare,      new List<Item>());
            mLibrary.Add(ItemRating.Epic,      new List<Item>());
            mLibrary.Add(ItemRating.Legendary, new List<Item>());

            List<Item> Items = RegisteredItem.UnlockedItemList;

            for (int i = 0; i < Items.Count; ++i)
            {
                if (Items[i] == null) continue;

                var item = Instantiate(Items[i], ItemStateSaver.Instance.transform);
                    item.transform.position = new Vector2(-10f, 0f);

                mLibrary[item.Rating].Add(item);
            }
        }
        for (int invokeCount = 0; invokeCount < 4; invokeCount++)
        {
            if (mLibrary[(ItemRating)invokeCount].Count == 0)
            {
                RevisionProbablity(mProbabilityArray[invokeCount]);
            }
        }
    }
    [ContextMenu("AAA")]
    private void AAA()
    {
        ItemUnlock(RegisteredItem.LockedItemList[0]);
    }

    public Item GetRandomItem()
    {
        float sum = 0f;
        float probability = Random.value;

        for (int invokeCount = 0; invokeCount < 4; invokeCount++)
        {
            ItemRating currentRating = (ItemRating)invokeCount;

            // 현재 등급의 아이템에서 반환할 수 있는 아이템이 없다면
            if (mLibrary[currentRating].Count == 0)
            {
                // 반복 중단
                continue;
            }
            sum += mProbabilityArray[invokeCount];

            // 반환할 아이템의 등급이 정해졌다면
            if (probability <= sum)
            {
                int index = Random.Range(0, mLibrary[currentRating].Count);

                // 그 등급의 아이템 중 무작위 아이템 반환
                var item = mLibrary[currentRating][index];
                           mLibrary[currentRating].RemoveAt(index);

                // 반환하기 전에, 리스트가 비어있다면
                if (mLibrary[currentRating].Count == 0)
                {
                    RevisionProbablity(mProbabilityArray[invokeCount]);
                }
                return item;
            }
        }
        return null;
    }

    public void ItemBoxReset()
    {
        mLibrary[ItemRating.Common   ].Clear();
        mLibrary[ItemRating.Rare     ].Clear();
        mLibrary[ItemRating.Epic     ].Clear();
        mLibrary[ItemRating.Legendary].Clear();

        var Items = _UnlockedItemListForTest;

        for (int i = 0; i < Items.Count; ++i)
        {
            if (Items[i] == null) {
                continue;
            }
            var item = Items[i];
            
            if (!ItemStateSaver.Instance.IsSavedItem(Items[i], out item))
            {
                item = Instantiate(Items[i], ItemStateSaver.Instance.transform);
                item.transform.position = new Vector2(-10f, 0);
            }
            mLibrary[item.Rating].Add(item);
        }
        for (int invokeCount = 0; invokeCount < 4; invokeCount++)
        {
            if (mLibrary[(ItemRating)invokeCount].Count == 0)
            {
                RevisionProbablity(mProbabilityArray[invokeCount]);
            }
        }
    }

    public Item GetRandomItem(ItemRating rating)
    {
        Item getItem = null;

        if (mLibrary[rating].Count != 0)
        {
            int itemIndex = Random.Range(0, mLibrary[rating].Count);

            getItem = mLibrary[rating][itemIndex];
                      mLibrary[rating].RemoveAt(itemIndex);
        }
        return getItem;
    }

    public void ItemUnlock(params Item[] unlockItems)
    {
        for (int i = 0; i < unlockItems.Length; i++)
        {
            Item item = _LockedItemListForTest.FirstOrDefault(o => o.GetType().Equals(unlockItems[i].GetType()));

            if (item != null)
            {
                item = Instantiate(item, ItemStateSaver.Instance.transform);
                item.transform.position = new Vector2(-10f, 0);

                mLibrary[item.Rating].Add(item);

                _UnlockedItemListForTest.Add(item);

                for (int j = 0; j < _LockedItemListForTest.Count; j++)
                {
                    if (_LockedItemListForTest[i].GetType().Equals(item.GetType()))
                    {

                        _LockedItemListForTest.RemoveAt(i); break;
                    }
                }
                ItemUnlockEvent?.Invoke(item);
            }
        }
    }

    public List<Item> GetUnlockedItemListForTest()
    {
        return _UnlockedItemListForTest;
    }
    public List<Item> GetLockedItemListForTest()
    {
        return _LockedItemListForTest;
    }
    private void RevisionProbablity(float selectedProbablity)
    {
        int division =
            mLibrary.ToList().Count(o => o.Value.Count > 0);

        float additive =
            selectedProbablity / division;

        for (int i = 0; i < 4; i++)
        {
            // 반환할 수 있는 나머지 등급들의 확률을 보정한다
            if (mLibrary[(ItemRating)i].Count > 0)
            {
                mProbabilityArray[i] += additive;
            }
        }
    }

    private void LoadItemListForTest()
    {
        if (!ItemStateSaver.Instance.LoadUnlockedItemListForTest(out _UnlockedItemListForTest))
        {
            _UnlockedItemListForTest = RegisteredItem.UnlockedItemList;
        }
        if (!ItemStateSaver.Instance.LoadLockedItemListForTest(out _LockedItemListForTest))
        {
            _LockedItemListForTest = RegisteredItem.LockedItemList;
        }
    }
}

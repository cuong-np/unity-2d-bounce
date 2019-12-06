using System.Collections.Generic;
using UnityEngine;

public class LeaderBoard : MonoBehaviour
{
    public static LeaderBoard instance;
    public GameObject newRecordObj;
    [HideInInspector]
    public Board[] listBoard;

    bool isShow = false;
    bool oldStateNewRecord = false;

    void Awake()
    {
        if (instance)
            GameObject.Destroy(this);
        else
            instance = this;
    }
    void Start()
    {
        oldStateNewRecord = newRecordObj.activeSelf;
    }

    void UpdateLeaderBoardUI(List<User> list)
    {
        for(int i=0;i< listBoard.Length;i++)
        {
            listBoard[i].UpdateText(list[i].name, list[i].score);
        }
        oldStateNewRecord = newRecordObj.activeSelf;
        newRecordObj.SetActive(false);
        transform.position = new Vector3(transform.position.x, transform.position.y, 100);
        isShow = true;
    }

    void Show()
    {
        FireBaseDatabase.instance.GetTopScore(4, UpdateLeaderBoardUI);
    }

    void Hide()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, -100);
        isShow = false;
        newRecordObj.SetActive(oldStateNewRecord);

    }

    public void OnClickRankButton()
    {
        if (isShow)
            Hide();
        else
            Show();
    }
}

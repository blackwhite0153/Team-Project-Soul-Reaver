using System.Collections.Generic;

// ���� ���� ������ ��� Ŭ����
public class Mail
{
    public bool isCanReceive = false;   // ���� ������ �������� ����

    public string Title;    // ���� ����
    public string Content;  // ���� ����
    public string InDate;   // ���� ���� ID (�ڳ����� ����)
    public string Author;   // ���� ���� ���

    // ���� ���� ������ ��� (�����۸�, ����)
    public Dictionary<string, int> mailReward = new Dictionary<string, int>();

    // ����� ����� ���� ToString �������̵�
    public override string ToString()
    {
        string result = string.Empty;
        result += $"Title : {Title}\n";
        result += $"Content : {Content}\n";
        result += $"InDate : {InDate}\n";
        result += $"Author : {Author}\n";

        if (isCanReceive)
        {
            result += "���� ������\n";

            foreach (string itemKey in mailReward.Keys)
            {
                result += $"| {itemKey} : {mailReward[itemKey]}��\n";
            }
        }
        else
        {
            result += "�������� �ʴ� ���� �������Դϴ�.";
        }

        return result;
    }
}
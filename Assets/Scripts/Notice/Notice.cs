using System;

// ���� ���� ���� ������ ��� Ŭ����
public class Notice
{
    public string Title;
    public string Content;
    public DateTime PostingDate;
    public string ImageKey;
    public string InDate;
    public string UUid;
    public string LinkUrl;
    public bool IsPublic;
    public string LinkButtonName;
    public string Author;

    // ����� ����� ���� ToString �������̵�
    public override string ToString()
    {
        return $"Title : {Title}\n" +
        $"Content : {Content}\n" +
        $"PostingDate : {PostingDate}\n" +
        $"ImageKey : {ImageKey}\n" +
        $"InDate : {InDate}\n" +
        $"UUid : {UUid}\n" +
        $"LinkUrl : {LinkUrl}\n" +
        $"IsPublic : {IsPublic}\n" +
        $"LinkButtonName : {LinkButtonName}\n" +
        $"Author : {Author}\n";
    }
}
using libMultiTerm.Plugin.Models;
using System.Collections;
using System.Collections.Specialized;
using Terminal.Gui;

public class TUIPayload : IListDataSource {
    private List<UserDefinedPayload> _payloads;
    private int? markedIndex;

    public TUIPayload(List<UserDefinedPayload> payloads) {
        _payloads = payloads;
    }

    public int Count => _payloads.Count();

    public int Length => _payloads.Count();

    public bool SuspendCollectionChangedEvent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public event NotifyCollectionChangedEventHandler CollectionChanged;

    public void Dispose() {
        _payloads.Clear();
    }

    public bool IsMarked(int item) {
        if (markedIndex is null) {
            return false;
        }

        return markedIndex == item;
    }

    public void Render(ListView listView, bool selected, int item, int col, int line, int width, int start = 0) {
        throw new NotImplementedException();
    }

    public void SetMark(int item, bool value) {
        throw new NotImplementedException();
    }

    public IList ToList() {
        throw new NotImplementedException();
    }
}
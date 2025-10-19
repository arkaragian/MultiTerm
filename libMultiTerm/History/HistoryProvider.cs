/// <summary>
/// A class that manages and provides hitory completions
/// </summary>
public class HistoryProvider<T> {
    private List<T> _historyItems;

    /// <summary>
    /// The current index of the history.
    /// </summary>
    private int _historyIndex = -1;

    public HistoryProvider() {
        _historyItems = new(capacity: 128);
    }

    public void AddItem(T item) {
        if (_historyItems.Count() is 0) {
            _historyItems.Add(item);
            return;
        }

        T prev = _historyItems.Last();
        bool areSame = item.Equals(prev);

        if (areSame) {
            return;
        }


        if (_historyItems.Count() > 128) {
            _historyItems.RemoveAt(0);
        }

        _historyItems.Add(item);

        _historyIndex = _historyItems.Count() - 1;

    }

    /// <summary>
    /// Gets the previous(most ancient) history item. Null indicates that we
    /// have reached the end of history.
    /// </summary>
    public T? Back() {
        if (_historyItems is null) {
            return default(T);
        }

        if (_historyItems.Count() is 0) {
            return default(T);
        }


        _historyIndex--;

        if (_historyIndex < 0) {
            _historyIndex = 0;
        }

        T result = _historyItems[_historyIndex];

        return result;
    }

    /// <summary>
    /// Gets the next(most recent) history item. Null indicates that
    /// we have reaced the start of history.
    /// </summary>
    public T? Next() {
        if (_historyItems is null) {
            return default(T);
        }

        _historyIndex++;

        if (_historyIndex > _historyItems.Count() - 1) {
            _historyIndex = _historyItems.Count() - 1;
        }

        return _historyItems[_historyIndex];


        // _history_index--;
        // if (_history_index < 0) {
        //     _history_index = 0;
        //     e.Handled = true;
        // }
        //
        // if (_history.Count() is 0) {
        //     e.Handled = true;
        //     return;
        // }
        //
        // if (_history_index > _history.Count - 1) {
        //     Input.Text = _history.Last();
        //     _history_index = _history.Count - 1;
        // } else {
        //     Input.Text = _history[_history_index];
        // }
    }


}
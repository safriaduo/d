using System.Collections;
using System.Collections.Generic;
using Dawnshard.Network;
using Dawnshard.Presenters;
using Dawnshard.Views;
using UnityEngine;

public class LogFactory : MonoBehaviour
{
    [SerializeField] private LogView logPrefab;
    [SerializeField] private SourceCardLogView sourceCardLogPrefab;
    [SerializeField] private ExtendedLogView extendedLogPrefab;
    private Transform extendedLogParentDefault;

    public static LogFactory Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void SetDefaultExtendedLogParent(Transform parent)
    {
        extendedLogParentDefault = parent;
    }

    public ILogPresenter CreateSourceCardLog(LogModel logModel, Transform parent = null)
    {
        var view = Instantiate(sourceCardLogPrefab, parent, false);
        return new HoverLogPresenter(view, logModel);
    }

    public ILogPresenter CreateExtendedLog(LogModel logModel)
    {
        var view = Instantiate(extendedLogPrefab, extendedLogParentDefault, false);
        return new LogPresenter(view, logModel);
    }

    public ILogPresenter CreateLog(LogModel logModel, Transform parent = null)
    {
        var view = Instantiate(logPrefab, parent, false);
        return new LogPresenter(view, logModel);
    }
}
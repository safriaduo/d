using System.Collections;
using System.Collections.Generic;
using Dawnshard.Network;
using TMPro;
using UnityEngine;

public class PostBattleBattlePassView : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private BattlePassView battlePassView;
    [SerializeField] private TMP_Text expGainedText;
    private BattlePassModel battlePassModel;

    private void SetBaseExp()
    {
        PopulateBattlePassView(battlePassModel);
    }
    public IEnumerator ExpUpCoroutine(int experience)
    {
        if(battlePassModel.CurrentExp>=battlePassModel.GetLevelTotalExp(battlePassModel.levels[^1]))
            yield break;
        SetBaseExp();
        expGainedText.text = $"+ {experience.ToString()} XP";
        List<int> expToNextLevels = CalculateExpToNextLevels();
        yield return new WaitForSeconds(1f);
        foreach (var expToNextLevel in expToNextLevels)
        {
            if (expToNextLevel > experience)
            {
                AddExperience(experience);
                yield return new WaitForSeconds(1f);
                break;
            }
            AddExperience(expToNextLevel);
            yield return new WaitForSeconds(1f);
            experience -= expToNextLevel;
            battlePassView.Refresh(true);
            yield return new WaitForSeconds(2f);
        }
    }

    private List<int> CalculateExpToNextLevels()
    {
        var currentUnlockedLevel = battlePassModel.GetNextUnlockableLevel();
        var expToNextLevels = battlePassModel.levels.GetRange(currentUnlockedLevel, battlePassModel.levels.Count - currentUnlockedLevel).ConvertAll(levelModel => levelModel.LevelUpExperience);
        expToNextLevels[0] -= battlePassModel.CurrentExp-battlePassModel.GetLevelTotalExp(battlePassModel.levels[currentUnlockedLevel - 1]);
        return expToNextLevels;
    }

    public void Show()
    {
        if(canvas.activeSelf)
            return;
        canvas.gameObject.SetActive(true);
        canvasGroup.alpha = 1;
        //TODO: get a specific battlepass
        battlePassModel = BattlePassAPI.BattlePass;
    }

    public IEnumerator ShowCoroutine(string battlePassId, int battlePassExp)
    {
        battlePassModel = BattlePassAPI.BattlePass;
        if(canvas.activeSelf || battlePassModel.CurrentExp>=battlePassModel.GetLevelTotalExp(battlePassModel.levels[^1]))
            yield break;
        canvasGroup.alpha = 0;
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += .02f;
            yield return new WaitForEndOfFrame();
        }
        canvas.gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (!canvas.activeSelf)
            return;
        canvas.gameObject.SetActive(false);
    }

    public IEnumerator HideCoroutine()
    {
        if (!canvas.activeSelf)
            yield break;
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= .02f;
            yield return new WaitForEndOfFrame();
        }
        canvas.gameObject.SetActive(false);
    }

    private void PopulateBattlePassView(BattlePassModel battlePassModel)
    {
        battlePassView.Initialize(battlePassModel, true);
    }

    private void AddExperience(int experience)
    {
        if(battlePassModel.CurrentExp+experience>=battlePassModel.GetLevelTotalExp(battlePassModel.levels[^1]))
            battlePassView.AddExperience(battlePassModel.GetLevelTotalExp(battlePassModel.levels[^1])-battlePassModel.CurrentExp);
        battlePassView.AddExperience(experience);
    }
}

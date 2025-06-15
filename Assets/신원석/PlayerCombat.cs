using System.Collections;
using UnityEngine;

public class PlayerCombat
{
    private PlayerStateMachine player;
    private Coroutine comboCoroutine;
    private bool nextComboQueued = false;
    private int attackCount = 0;
    private float comboWindow = 0.5f;

    public PlayerCombat(PlayerStateMachine player)
    {
        this.player = player;
    }

    public void StartAttack()
    {
        attackCount = 1;
        nextComboQueued = false;
        player.SetIsAttackTrue();
        player.AnimHandler.SetAttackCount(attackCount);
        player.AnimHandler.SetAttackBool(true);
        StartComboWindow();
    }




    public void TryQueueNextCombo()
    {
        if (attackCount < 4)
            nextComboQueued = true;
    }

    public void OnAnimationEnd()
    {
        if (nextComboQueued && attackCount < 4)
        {
            attackCount++;
            nextComboQueued = false;
            player.SetIsAttackTrue();
            player.AnimHandler.SetAttackCount(attackCount);
            player.AnimHandler.SetAttackBool(true);
            StartComboWindow();
        }
        else
        {   
            player.SetIsAttackFalse();
            player.AnimHandler.SetAttackBool(false);
        }
      
    }

    private void StartComboWindow()
    {
        if (comboCoroutine != null)
            player.StopCoroutine(comboCoroutine);
        comboCoroutine = player.StartCoroutine(ComboInputWindow());
    }

    private IEnumerator ComboInputWindow()
    {
        yield return new WaitForSeconds(0.1f);

        float timer = comboWindow;
        while (timer > 0)
        {
            if (Input.GetMouseButtonDown(0)) // or Fusion 방식 입력
            {
                TryQueueNextCombo();
                yield break;
            }

            timer -= Time.deltaTime;
            yield return null;
        }
    }
}
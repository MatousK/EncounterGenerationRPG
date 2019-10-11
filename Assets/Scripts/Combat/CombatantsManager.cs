using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
class CombatantsManager : MonoBehaviour
{
    public List<Enemy> Enemies = new List<Enemy>();
    public List<Character> PlayerCharacters = new List<Character>();

    public IEnumerable<CombatantBase> GetOpponentsFor(CombatantBase combatant, bool onlyAlive = false, bool onlySelected = false)
    {
        IEnumerable<CombatantBase> opponents = null;
        if (combatant is Character)
        {
            return GetEnemies(onlyAlive, onlySelected);
        }
        else if (combatant is Enemy)
        {
            return GetPlayerCharacters(onlyAlive, onlySelected);
        }
        if (opponents == null)
        {
            Debug.Assert(false, "Asked for opponents for unknown combatant.");
            return null;
        }
        return null;
    }

    public IEnumerable<Character> GetPlayerCharacters(bool onlyAlive = false, bool onlySelected = false)
    {
        return PlayerCharacters.Where(opponent => (!onlyAlive || !opponent.IsDown) && (!onlySelected || opponent.GetComponent<SelectableObject>()?.IsSelected == true));
    }

    public IEnumerable<Enemy> GetEnemies(bool onlyAlive = false, bool onlySelected = false)
    {
        return Enemies.Where(opponent => (!onlyAlive || !opponent.IsDown) && (!onlySelected || opponent.GetComponent<SelectableObject>()?.IsSelected == true));
    }
}

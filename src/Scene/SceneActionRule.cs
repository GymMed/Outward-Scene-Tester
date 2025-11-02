using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardSceneTester.Scene
{
    public class SceneActionRule
    {
        public string ID { get; set; }
        public Action ProvidedFunction { get; set; }
        public HashSet<AreaManager.AreaEnum> Areas { get; set; }
        public HashSet<AreaManager.AreaEnum> VisitedAreas { get; set; }

        public SceneActionRule(Action providedFunction, HashSet<AreaManager.AreaEnum> areas)
        {
            ID = UID.Generate().Value;
            ProvidedFunction = providedFunction;
            Areas = areas ?? new HashSet<AreaManager.AreaEnum>();
            VisitedAreas = new HashSet<AreaManager.AreaEnum>();
        }

        public SceneActionRule(string id, Action providedFunction, HashSet<AreaManager.AreaEnum> areas)
        {
            ID = id;
            ProvidedFunction = providedFunction;
            Areas = areas ?? new HashSet<AreaManager.AreaEnum>();
            VisitedAreas = new HashSet<AreaManager.AreaEnum>();
        }

        public HashSet<AreaManager.AreaEnum> GetUnvisitedAreas() 
        { 
            HashSet<AreaManager.AreaEnum> unvisitedAreas = new HashSet<AreaManager.AreaEnum>();

            foreach(AreaManager.AreaEnum area in Areas)
            {
                if(!VisitedAreas.TryGetValue(area, out AreaManager.AreaEnum foundArea))
                {
                    unvisitedAreas.Add(area);
                }
            }

            return unvisitedAreas;
        }

        public bool IsMatching()
        {
            HashSet<AreaManager.AreaEnum> unvisitedAreas = GetUnvisitedAreas();

            foreach (AreaManager.AreaEnum area in unvisitedAreas) {
                if (AreaManager.Instance.CurrentArea.ID == AreaManager.Instance.GetArea(area).ID)
                    return true;
            }

            return false;
        }

        public void ValidateRule()
        {
            if (!this.IsMatching() || ProvidedFunction == null)
                return;

            ProvidedFunction.Invoke();
            VisitedAreas.Add(GetAreaEnumFromArea(AreaManager.Instance.CurrentArea));
        }

        public static AreaManager.AreaEnum GetAreaEnumFromArea(Area area)
        {
            return (AreaManager.AreaEnum)area.ID;
        }
    }
}

using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace HCI_PROJEKA_1
{
    public class GoalViewModel
    {
        public ObservableCollection<GoalModel> AllGoals { get; set; }
        public ObservableCollection<GoalModel> Goals { get; set; }

        public GoalViewModel()
        {
            int balanceID = UserModel.GetBalanceIDForUser(LoginViewModel.LoggedInUsername);
            AllGoals = new ObservableCollection<GoalModel>(GoalModel.GetAllGoals(balanceID));
            Goals = new ObservableCollection<GoalModel>(AllGoals);
        }

        public void AddGoal(string name, decimal value, int instalments)
        {
            var goal = new GoalModel
            {
                GoalName = name,
                GoalValue = value,
                Instalments = instalments,
                BalanceID = UserModel.GetBalanceIDForUser(LoginViewModel.LoggedInUsername)
            };

            GoalModel.AddGoal(goal);

            int balanceID = UserModel.GetBalanceIDForUser(LoginViewModel.LoggedInUsername);
            AllGoals.Clear();
            foreach (var g in GoalModel.GetAllGoals(balanceID))
            {
                AllGoals.Add(g);
            }

            Goals.Clear();
            foreach (var g in AllGoals)
            {
                Goals.Add(g);
            }
        }

        public void DeleteGoal(GoalModel goal)
        {
            GoalModel.DeleteGoal(goal.GoalID);
            AllGoals.Remove(goal);
            Goals.Remove(goal);
        }

        public void UpdateGoal(GoalModel oldGoal, GoalModel newGoal)
        {
            GoalModel.UpdateGoal(newGoal);
            int indexInAllGoals = AllGoals.IndexOf(oldGoal);
            if (indexInAllGoals >= 0)
            {
                AllGoals[indexInAllGoals] = newGoal;
            }

            int indexInGoals = Goals.IndexOf(oldGoal);
            if (indexInGoals >= 0)
            {
                Goals[indexInGoals] = newGoal;
            }
        }

        public void SearchGoals(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                Goals.Clear();
                foreach (var goal in AllGoals)
                {
                    Goals.Add(goal);
                }
                return;
            }

            var filteredGoals = AllGoals
                .Where(g => g.GoalName != null &&
                            g.GoalName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            Goals.Clear();
            foreach (var goal in filteredGoals)
            {
                Goals.Add(goal);
            }
        }
    }
}

using System;
using System.Collections;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	/// <summary>
	/// A generic project contains a collection of tasks.  It will execute them in the specified order.  It is possible to have multiple tasks of the same type.
	/// <code>
	/// <![CDATA[
	/// <workflow name="foo">
	///		<tasks>
	///			<sourcecontrol type="cvs"></sourcecontrol>
	///			<build type="nant"></build>
	///		</tasks>
	///		<state type="state"></state>
	/// </workflow>
	/// ]]>
	/// </code>
	/// </summary>
	[ReflectorType("workflow")]
	public class Workflow : ProjectBase, IProject
	{
		private IList _tasks = new ArrayList();
		private WorkflowResult _currentIntegrationResult;

		[ReflectorCollection("tasks", InstanceType = typeof(ArrayList))]
		public IList Tasks
		{
			get { return _tasks; }
			set { _tasks = value; }
		}

		public IntegrationResult CurrentIntegration
		{
			get { return _currentIntegrationResult; }
		}

		public IntegrationResult RunIntegration(BuildCondition buildCondition)
		{
			_currentIntegrationResult = new WorkflowResult();

			foreach (ITask task in Tasks)
			{
				try 
				{ 
					RunTask(buildCondition, task); 
				}
				catch (CruiseControlException ex) 
				{
					_currentIntegrationResult.ExceptionResult = ex;
					_currentIntegrationResult.Status = IntegrationStatus.Exception;
				}
			}
			return _currentIntegrationResult;
		}

		private void RunTask(BuildCondition buildCondition, ITask task)
		{
			if (buildCondition == BuildCondition.ForceBuild || task.ShouldRun(_currentIntegrationResult))
			{
				task.Run(_currentIntegrationResult);
			}
		}
		
		public IntegrationStatus GetLatestBuildStatus()
		{
			return _currentIntegrationResult.Status;
		}

		public int MinimumSleepTimeMillis 
		{ 
			get { return 0; }
		}

		public ProjectActivity CurrentActivity 
		{
			get { return ProjectActivity.Unknown; }
		}

		public string WebURL 
		{ 
			get { return ""; }
		}
	}
}

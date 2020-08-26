#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

namespace NinjaTrader.NinjaScript.Strategies
{
	public class RenkoTrendADX : Strategy
	{
		private Stochastics Stochastics1;
		private Stochastics Stochastics2;
		private EMA EMA1;
		private EMA EMA2;
		private ADX ADX1;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description					= @"Enter the description for your new custom Strategy here.";
				Name						= "RenkoTest2TrendADXTimeCode";
				Calculate					= Calculate.OnBarClose;
				EntriesPerDirection				= 1;
				EntryHandling					= EntryHandling.AllEntries;
				IsExitOnSessionCloseStrategy			= true;
				ExitOnSessionCloseSeconds			= 30;
				IsFillLimitOnTouch				= false;
				MaximumBarsLookBack				= MaximumBarsLookBack.TwoHundredFiftySix;
				OrderFillResolution				= OrderFillResolution.Standard;
				Slippage					= 0;
				StartBehavior					= StartBehavior.WaitUntilFlat;
				TimeInForce					= TimeInForce.Gtc;
				TraceOrders					= false;
				RealtimeErrorHandling				= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling				= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade				= 20;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				IsInstantiatedOnEachOptimizationIteration	= true;
				StopLoss					= 8;
				TakeProfit					= 4;
				MAPeriod					= 200;
				StochD						= 7;
				StochK						= 14;
				StochS						= 3;
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{				
				Stochastics1			= Stochastics(Close, Convert.ToInt32(StochD), Convert.ToInt32(StochK), Convert.ToInt32(StochS));
				Stochastics2			= Stochastics(Close, 7, 14, 3);
				EMA1				= EMA(Close, 5);
				EMA2				= EMA(Close, Convert.ToInt32(MAPeriod));
				ADX1				= ADX(Close, 14);
				Stochastics1.Plots[0].Brush 	= Brushes.DodgerBlue;
				Stochastics1.Plots[1].Brush 	= Brushes.Goldenrod;
				Stochastics2.Plots[0].Brush 	= Brushes.DodgerBlue;
				Stochastics2.Plots[1].Brush 	= Brushes.Goldenrod;
				EMA1.Plots[0].Brush 		= Brushes.Goldenrod;
				EMA2.Plots[0].Brush 		= Brushes.Goldenrod;
				ADX1.Plots[0].Brush 		= Brushes.DarkCyan;
				AddChartIndicator(Stochastics1);
				AddChartIndicator(Stochastics2);
				AddChartIndicator(EMA1);
				AddChartIndicator(EMA2);
				AddChartIndicator(ADX1);
				SetProfitTarget("", CalculationMode.Ticks, TakeProfit);
				SetStopLoss("", CalculationMode.Ticks, StopLoss, false);
			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0) 
				return;

			if (CurrentBars[0] < 1)
				return;

			 // Set 1
			if ((Stochastics1.D[0] > 50)
				 && (Stochastics1.D[0] > Stochastics2.D[1])
				 && (CrossAbove(Close, EMA1, 1))
				 && (EMA2[0] > EMA2[1])
				 && (ADX1[0] > ADX1[1]))
			{
				EnterLongLimit(Convert.ToInt32(DefaultQuantity), Close[0], "");
			}
			
			 // Set 2
			if ((Stochastics1.D[0] < 50)
				 && (Stochastics1.D[0] < Stochastics2.D[1])
				 && (CrossBelow(Close, EMA1, 1))
				 && (EMA2[0] < EMA2[1])
				 && (ADX1[0] > ADX1[1]))
			{
				EnterLongLimit(Convert.ToInt32(DefaultQuantity), Close[0], "");
			}
			
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="StopLoss", Order=1, GroupName="Parameters")]
		public int StopLoss
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="TakeProfit", Order=2, GroupName="Parameters")]
		public int TakeProfit
		{ get; set; }

		[NinjaScriptProperty]
		[Range(0, int.MaxValue)]
		[Display(Name="MAPeriod", Order=3, GroupName="Parameters")]
		public int MAPeriod
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="StochD", Order=4, GroupName="Parameters")]
		public int StochD
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="StochK", Order=5, GroupName="Parameters")]
		public int StochK
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="StochS", Order=6, GroupName="Parameters")]
		public int StochS
		{ get; set; }
		#endregion

	}
}

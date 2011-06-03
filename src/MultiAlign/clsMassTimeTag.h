#pragma once

namespace MultiAlignEngine
{
	/// <summary>
	/// Class used for mz, net patterns for matching.
	/// </summary>
		[System::Serializable]
	public __gc class clsMassTimeTag : public System::IComparable
	{
	public:
		int CompareTo(System::Object* obj)
		{ 
			clsMassTimeTag *u = dynamic_cast<clsMassTimeTag *>(obj); 
			if (mdblMass == u->mdblMass) 
			{
				if (mdblNET == u->mdblNET)
				{
					//TODO: Add drift time.
					return 0;
				}
				if (mdblNET < u->mdblNET)
				{
					//TODO: Add drift time.
					return -1; 
				}
				return 1; 
			} 
			if (mdblMass < u->mdblMass) 
				return -1; 
			return 1; 
		} 
		double mdblMass;			/// Monoisotopic mass.
		double mdblNET;				/// NET.
		double mdblDriftTime;		/// Calculated drift time (IMS).
		int    mintID; 
		bool   mblnMSMS; 
		int  ChargeState;
		clsMassTimeTag():mdblMass(0), mdblNET(0), mintID(-1), mblnMSMS(false), mdblDriftTime(0) {}
		clsMassTimeTag(double mass, double net, double driftTime):mdblMass(mass), mdblNET(net), mdblDriftTime(driftTime), mintID(-1), mblnMSMS(false){} 
		clsMassTimeTag(double mass, double net, double driftTime, int id):mdblMass(mass), mdblNET(net), mdblDriftTime(driftTime), mintID(id), mblnMSMS(false) {} 
		clsMassTimeTag(double mass, double net, double driftTime, int id, bool isMSMS):mdblMass(mass), mdblNET(net), mdblDriftTime(driftTime), mintID(id), mblnMSMS(isMSMS) {} 
		inline void Set(double mass, double net, double driftTime, int id, bool isMSMS) { mdblMass = mass; mdblNET = net; mdblDriftTime = driftTime;  mintID = id; mblnMSMS = isMSMS;}
	};
}
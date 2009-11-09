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
					return 0;
				if (mdblNET < u->mdblNET)
					return -1; 
				return 1; 
			} 
			if (mdblMass < u->mdblMass) 
				return -1; 
			return 1; 
		} 
		double mdblMass; 
		double mdblNET;
		int mintID; 
		bool mblnMSMS; 
		clsMassTimeTag():mdblMass(0), mdblNET(0), mintID(-1), mblnMSMS(false){}
		clsMassTimeTag(double mass, double net):mdblMass(mass), mdblNET(net), mintID(-1), mblnMSMS(false){} 
		clsMassTimeTag(double mass, double net, int id):mdblMass(mass), mdblNET(net), mintID(id), mblnMSMS(false) {} 
		clsMassTimeTag(double mass, double net, int id, bool isMSMS):mdblMass(mass), mdblNET(net), mintID(id), mblnMSMS(isMSMS) {} 
		inline void Set(double mass, double net, int id, bool isMSMS) { mdblMass = mass; mdblNET = net; mintID = id; mblnMSMS = isMSMS;}
	};
}
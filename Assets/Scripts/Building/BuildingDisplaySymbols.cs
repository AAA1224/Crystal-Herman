using Herman;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDisplaySymbols: MarketplaceDisplaySymbols
{
    public override Marketplace getMarketplace()
    {
        Building building = GetComponentInParent<Building>();
        return building.Marketplace;
    }
}

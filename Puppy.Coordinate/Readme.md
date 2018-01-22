![Logo](favicon.ico)
# Puppy.Coordinate
> Project Created by [**Top Nguyen**](http://topnguyen.net)

## Support

- Support Calculate Distance by Algorithms: Spherical law of cosines, Haversine or Geographical

- Support Calculate Derived Position

- Support Cluster (K-Means)

- Support Find Fastest Route/Trip: A -> Z or Round Trip (A -> A)

### Example Use

```csharp
    public void ClusterTest()
    {
        var coordinates = new List<Coordinate.Models.Coordinate>
        {
            new Coordinate.Models.Coordinate(106.66691070000002, 10.8002712),
            new Coordinate.Models.Coordinate(106.66909299999998, 10.8001182),
            new Coordinate.Models.Coordinate(106.6677267, 10.7993045),
            new Coordinate.Models.Coordinate(106.66926460000002, 10.798618),
            new Coordinate.Models.Coordinate(106.6654264, 10.7978616),
            new Coordinate.Models.Coordinate(106.66459699999996, 10.7991835),
            new Coordinate.Models.Coordinate(106.66446250000001, 10.7985912),
            new Coordinate.Models.Coordinate(106.6644063, 10.7982545),
            new Coordinate.Models.Coordinate(106.66749460000005, 10.7972986),
            new Coordinate.Models.Coordinate(106.66693399999997, 10.7990973)
        };

        GeoClustering gc = new GeoClustering();

        var cluster = gc.Cluster(coordinates, 3);
    }

    public void FastestTripTest()
    {
        // 31 Trương phước phan, 10.7756192, 106.6237788

        // 435 hoàng văn thụ hồ chí minh, 10.7975076, 106.6558153

        // 60 cộng hoà, 10.8011653, 106.657283

        // 10 Tân kỳ tân quý hồ chí minh, 10.7922362, 106.605514

        // 30 âu cơ hồ chí minh, 10.7877314, 106.64095250000003

        var coordinates = new List<Coordinate.Models.Coordinate>
        {
            new Coordinate.Models.Coordinate(106.6237788,10.7756192),
            new Coordinate.Models.Coordinate(106.6558153,10.7975076),
            new Coordinate.Models.Coordinate(106.657283,10.8011653) ,
            new Coordinate.Models.Coordinate(106.605514,10.7922362),
            new Coordinate.Models.Coordinate(106.64095250000003,10.7877314)
        };

        // A to Z TRIP

        FastestAzTrip fastestAzTrip = new FastestAzTrip(coordinates);

        List<Coordinate.Models.Coordinate> azTripCoordinates = fastestAzTrip.GetTrip();

        // ROUND TRIP

        FastestRoundTrip fastestRoundTrip = new FastestRoundTrip(coordinates);

        List<Coordinate.Models.Coordinate> roundTripCoordinates = fastestRoundTrip.GetTrip();
    }

    public static void FastestOptimizeTest()
    {
        // 31 Trương phước phan, 10.7756192, 106.6237788

        // 435 hoàng văn thụ hồ chí minh, 10.7975076, 106.6558153

        // 60 cộng hoà, 10.8011653, 106.657283

        // 10 Tân kỳ tân quý hồ chí minh, 10.7922362, 106.605514

        // 30 âu cơ hồ chí minh, 10.7877314, 106.64095250000003

        var coordinates = new List<Coordinate.Models.Coordinate>
        {
            new Coordinate.Models.Coordinate(106.6237788,10.7756192),
            new Coordinate.Models.Coordinate(106.6558153,10.7975076),
            new Coordinate.Models.Coordinate(106.657283,10.8011653) ,
            new Coordinate.Models.Coordinate(106.605514,10.7922362),
            new Coordinate.Models.Coordinate(106.64095250000003,10.7877314)
        };

        // ROUND TRIP

        FastestTrip fastestRoundTrip = new FastestTrip(coordinates, FastestTrip.FastestTripMode.RoundTrip);

        List<Coordinate.Models.Coordinate> roundTrip = fastestRoundTrip.GetTrip();

        double totalRoundTripDistance = fastestRoundTrip.GetTotalDistance();

        double totalRoundTripDuration = fastestRoundTrip.GetTotalDuration();

        // A -> Z TRIP

        FastestTrip fastestAzTrip = new FastestTrip(coordinates, FastestTrip.FastestTripMode.AtoZ);

        List<Coordinate.Models.Coordinate> azTrip = fastestAzTrip.GetTrip();

        double totalAzTripDistance = fastestAzTrip.GetTotalDistance();

        double totalAzTripDuration = fastestAzTrip.GetTotalDuration();
    }
```


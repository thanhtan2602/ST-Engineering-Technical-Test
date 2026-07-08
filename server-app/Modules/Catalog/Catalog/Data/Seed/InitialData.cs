namespace Catalog.Data.Seed
{
    public static class InitialData
    {
        // ── ProductTypes ───────────────────────────────────────────────────────────
        public static readonly Guid ApparelTypeId     = new("11111111-1111-1111-1111-000000000001");
        public static readonly Guid FootwearTypeId    = new("11111111-1111-1111-1111-000000000002");
        public static readonly Guid AccessoriesTypeId = new("11111111-1111-1111-1111-000000000003");

        // ── Categories ────────────────────────────────────────────────────────────
        public static readonly Guid ClothingCategoryId    = new("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1");
        public static readonly Guid ShoesCategoryId       = new("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2");
        public static readonly Guid AccessoriesCategoryId = new("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3");
        public static readonly Guid BagsCategoryId        = new("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4");
        public static readonly Guid SportswearCategoryId  = new("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5");

        // ── Brands ────────────────────────────────────────────────────────────────
        public static readonly Guid NikeBrandId       = new("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1");
        public static readonly Guid AdidasBrandId     = new("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2");
        public static readonly Guid PumaBrandId       = new("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3");
        public static readonly Guid NewBalanceBrandId = new("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb4");
        public static readonly Guid ConverseBrandId   = new("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb5");

        // ── AttributeDefinitions (15) ─────────────────────────────────────────────
        // Shared
        public static readonly Guid ColorAttrId      = new("cccccccc-cccc-cccc-cccc-000000000001");
        public static readonly Guid SizeAttrId       = new("cccccccc-cccc-cccc-cccc-000000000002");
        public static readonly Guid MaterialAttrId   = new("cccccccc-cccc-cccc-cccc-000000000003");
        public static readonly Guid GenderAttrId     = new("cccccccc-cccc-cccc-cccc-000000000004");
        public static readonly Guid SeasonAttrId     = new("cccccccc-cccc-cccc-cccc-000000000005");
        // Apparel
        public static readonly Guid FitAttrId        = new("cccccccc-cccc-cccc-cccc-000000000006");
        public static readonly Guid StyleAttrId      = new("cccccccc-cccc-cccc-cccc-000000000007");
        public static readonly Guid PatternAttrId    = new("cccccccc-cccc-cccc-cccc-000000000008");
        // Footwear
        public static readonly Guid ClosureAttrId    = new("cccccccc-cccc-cccc-cccc-000000000009");
        public static readonly Guid SoleAttrId       = new("cccccccc-cccc-cccc-cccc-000000000010");
        public static readonly Guid WaterproofAttrId = new("cccccccc-cccc-cccc-cccc-000000000011");
        // Accessories / Bags
        public static readonly Guid CapacityAttrId   = new("cccccccc-cccc-cccc-cccc-000000000012");
        public static readonly Guid StrapTypeAttrId  = new("cccccccc-cccc-cccc-cccc-000000000013");
        public static readonly Guid DimensionsAttrId = new("cccccccc-cccc-cccc-cccc-000000000014");
        public static readonly Guid WeightAttrId     = new("cccccccc-cccc-cccc-cccc-000000000015");

        // ─────────────────────────────────────────────────────────────────────────

        public static IEnumerable<Category> Categories =>
        [
            Category.Create(ClothingCategoryId,    "Clothing",    "clothing"),
            Category.Create(ShoesCategoryId,       "Shoes",       "shoes"),
            Category.Create(AccessoriesCategoryId, "Accessories", "accessories"),
            Category.Create(BagsCategoryId,        "Bags",        "bags"),
            Category.Create(SportswearCategoryId,  "Sportswear",  "sportswear"),
        ];

        public static IEnumerable<Brand> Brands =>
        [
            Brand.Create(NikeBrandId,       "Nike",        "nike"),
            Brand.Create(AdidasBrandId,     "Adidas",      "adidas"),
            Brand.Create(PumaBrandId,       "Puma",        "puma"),
            Brand.Create(NewBalanceBrandId, "New Balance", "new-balance"),
            Brand.Create(ConverseBrandId,   "Converse",    "converse"),
        ];

        public static IEnumerable<AttributeDefinition> AttributeDefinitions =>
        [
            // Shared
            AttributeDefinition.Create(ColorAttrId, "color", "Color", AttributeDataType.Enum, null,
                ["Black", "White", "Red", "Blue", "Green", "Navy", "Grey", "Beige", "Brown", "Pink"]),
            AttributeDefinition.Create(SizeAttrId, "size", "Size", AttributeDataType.Enum, null,
                ["XS", "S", "M", "L", "XL", "XXL", "36", "37", "38", "39", "40", "41", "42", "43", "44", "One Size"]),
            AttributeDefinition.Create(MaterialAttrId, "material", "Material", AttributeDataType.Text),
            AttributeDefinition.Create(GenderAttrId, "gender", "Gender", AttributeDataType.Enum, null,
                ["Men", "Women", "Unisex"]),
            AttributeDefinition.Create(SeasonAttrId, "season", "Season", AttributeDataType.Enum, null,
                ["Spring/Summer", "Autumn/Winter", "All Season"]),
            // Apparel
            AttributeDefinition.Create(FitAttrId, "fit", "Fit", AttributeDataType.Enum, null,
                ["Slim", "Regular", "Relaxed", "Oversized"]),
            AttributeDefinition.Create(StyleAttrId, "style", "Style", AttributeDataType.Enum, null,
                ["Casual", "Sport", "Formal", "Street"]),
            AttributeDefinition.Create(PatternAttrId, "pattern", "Pattern", AttributeDataType.Enum, null,
                ["Solid", "Striped", "Graphic", "Checkered"]),
            // Footwear
            AttributeDefinition.Create(ClosureAttrId, "closure", "Closure", AttributeDataType.Enum, null,
                ["Lace-up", "Slip-on", "Velcro", "Zip"]),
            AttributeDefinition.Create(SoleAttrId, "sole", "Sole Material", AttributeDataType.Text),
            AttributeDefinition.Create(WaterproofAttrId, "waterproof", "Waterproof", AttributeDataType.Boolean),
            // Accessories / Bags
            AttributeDefinition.Create(CapacityAttrId, "capacity", "Capacity (L)", AttributeDataType.Number),
            AttributeDefinition.Create(StrapTypeAttrId, "strap-type", "Strap Type", AttributeDataType.Enum, null,
                ["Backpack", "Shoulder", "Crossbody", "Tote", "Waist"]),
            AttributeDefinition.Create(DimensionsAttrId, "dimensions", "Dimensions (cm)", AttributeDataType.Text),
            AttributeDefinition.Create(WeightAttrId, "weight", "Weight (g)", AttributeDataType.Number),
        ];

        public static IEnumerable<ProductType> ProductTypes
        {
            get
            {
                var apparel = ProductType.Create(ApparelTypeId, "apparel", "Apparel");
                apparel.AttachAttribute(ColorAttrId,    isRequired: true,  displayOrder: 1);
                apparel.AttachAttribute(SizeAttrId,     isRequired: true,  displayOrder: 2);
                apparel.AttachAttribute(MaterialAttrId, isRequired: false, displayOrder: 3);
                apparel.AttachAttribute(GenderAttrId,   isRequired: false, displayOrder: 4);
                apparel.AttachAttribute(FitAttrId,      isRequired: false, displayOrder: 5);
                apparel.AttachAttribute(StyleAttrId,    isRequired: false, displayOrder: 6);
                apparel.AttachAttribute(PatternAttrId,  isRequired: false, displayOrder: 7);
                apparel.AttachAttribute(SeasonAttrId,   isRequired: false, displayOrder: 8);
                yield return apparel;

                var footwear = ProductType.Create(FootwearTypeId, "footwear", "Footwear");
                footwear.AttachAttribute(ColorAttrId,      isRequired: true,  displayOrder: 1);
                footwear.AttachAttribute(SizeAttrId,       isRequired: true,  displayOrder: 2);
                footwear.AttachAttribute(MaterialAttrId,   isRequired: false, displayOrder: 3);
                footwear.AttachAttribute(GenderAttrId,     isRequired: false, displayOrder: 4);
                footwear.AttachAttribute(ClosureAttrId,    isRequired: false, displayOrder: 5);
                footwear.AttachAttribute(SoleAttrId,       isRequired: false, displayOrder: 6);
                footwear.AttachAttribute(WaterproofAttrId, isRequired: false, displayOrder: 7);
                yield return footwear;

                var accessories = ProductType.Create(AccessoriesTypeId, "accessories", "Accessories");
                accessories.AttachAttribute(ColorAttrId,     isRequired: true,  displayOrder: 1);
                accessories.AttachAttribute(MaterialAttrId,  isRequired: false, displayOrder: 2);
                accessories.AttachAttribute(GenderAttrId,    isRequired: false, displayOrder: 3);
                accessories.AttachAttribute(CapacityAttrId,  isRequired: false, displayOrder: 4);
                accessories.AttachAttribute(StrapTypeAttrId, isRequired: false, displayOrder: 5);
                accessories.AttachAttribute(DimensionsAttrId, isRequired: false, displayOrder: 6);
                accessories.AttachAttribute(WeightAttrId,   isRequired: false, displayOrder: 7);
                yield return accessories;
            }
        }

        public static IEnumerable<Product> Products
        {
            get
            {
                // ── Footwear (8) ───────────────────────────────────────────────────
                var p = Make("5334c996-8457-4cf0-815c-ed2b77c4ff61",
                    "NIKE-AM90-BLK-42", "Nike Air Max 90 Black", "nike-air-max-90-black",
                    "Classic Nike Air Max 90 sneakers.", 129m, ShoesCategoryId, NikeBrandId, FootwearTypeId);
                p.SetAttribute(ColorAttrId, "Black"); p.SetAttribute(SizeAttrId, "42");
                p.SetAttribute(MaterialAttrId, "Mesh / Leather"); p.SetAttribute(GenderAttrId, "Men");
                p.SetAttribute(ClosureAttrId, "Lace-up"); p.SetAttribute(SoleAttrId, "Rubber");
                yield return p;

                p = Make("c67d6323-e8b1-4bdf-9a75-b0d0d2e7e914",
                    "ADIDAS-SS-WHT-40", "Adidas Stan Smith White", "adidas-stan-smith-white",
                    "Iconic Adidas Stan Smith sneakers.", 99m, ShoesCategoryId, AdidasBrandId, FootwearTypeId);
                p.SetAttribute(ColorAttrId, "White"); p.SetAttribute(SizeAttrId, "40");
                p.SetAttribute(MaterialAttrId, "Leather"); p.SetAttribute(GenderAttrId, "Unisex");
                p.SetAttribute(ClosureAttrId, "Lace-up"); p.SetAttribute(SoleAttrId, "Rubber");
                yield return p;

                p = Make("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee03",
                    "NIKE-REACT-BLU-41", "Nike React Infinity Run Blue", "nike-react-infinity-run-blue",
                    "Lightweight running shoe designed for endurance.", 159m, ShoesCategoryId, NikeBrandId, FootwearTypeId);
                p.SetAttribute(ColorAttrId, "Blue"); p.SetAttribute(SizeAttrId, "41");
                p.SetAttribute(MaterialAttrId, "Flyknit"); p.SetAttribute(GenderAttrId, "Men");
                p.SetAttribute(ClosureAttrId, "Lace-up"); p.SetAttribute(WaterproofAttrId, "false");
                yield return p;

                p = Make("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee04",
                    "ADIDAS-UB22-BLK-42", "Adidas Ultraboost 22 Black", "adidas-ultraboost-22-black",
                    "Premium running shoe with Boost cushioning.", 189m, ShoesCategoryId, AdidasBrandId, FootwearTypeId);
                p.SetAttribute(ColorAttrId, "Black"); p.SetAttribute(SizeAttrId, "42");
                p.SetAttribute(MaterialAttrId, "Primeknit"); p.SetAttribute(GenderAttrId, "Men");
                p.SetAttribute(ClosureAttrId, "Lace-up"); p.SetAttribute(WaterproofAttrId, "false");
                yield return p;

                p = Make("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee05",
                    "PUMA-RSX-WHT-41", "Puma RS-X White", "puma-rs-x-white",
                    "Retro-inspired chunky sneaker.", 109m, ShoesCategoryId, PumaBrandId, FootwearTypeId);
                p.SetAttribute(ColorAttrId, "White"); p.SetAttribute(SizeAttrId, "41");
                p.SetAttribute(MaterialAttrId, "Mesh / Synthetic"); p.SetAttribute(GenderAttrId, "Unisex");
                p.SetAttribute(ClosureAttrId, "Lace-up"); p.SetAttribute(SoleAttrId, "EVA Foam");
                yield return p;

                p = Make("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee06",
                    "NB-990V5-BLK-40", "New Balance 990v5 Black", "new-balance-990v5-black",
                    "Made in USA. Premium suede upper.", 175m, ShoesCategoryId, NewBalanceBrandId, FootwearTypeId);
                p.SetAttribute(ColorAttrId, "Black"); p.SetAttribute(SizeAttrId, "40");
                p.SetAttribute(MaterialAttrId, "Suede / Mesh"); p.SetAttribute(GenderAttrId, "Men");
                p.SetAttribute(ClosureAttrId, "Lace-up"); p.SetAttribute(SoleAttrId, "ENCAP Midsole");
                yield return p;

                p = Make("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee07",
                    "CV-CHUCK-BLK-39", "Converse Chuck Taylor All Star Black", "converse-chuck-taylor-all-star-black",
                    "The original canvas high-top sneaker.", 65m, ShoesCategoryId, ConverseBrandId, FootwearTypeId);
                p.SetAttribute(ColorAttrId, "Black"); p.SetAttribute(SizeAttrId, "39");
                p.SetAttribute(MaterialAttrId, "Canvas"); p.SetAttribute(GenderAttrId, "Unisex");
                p.SetAttribute(ClosureAttrId, "Lace-up"); p.SetAttribute(SoleAttrId, "Vulcanized Rubber");
                yield return p;

                p = Make("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee08",
                    "NIKE-AF1-WHT-42", "Nike Air Force 1 White", "nike-air-force-1-white",
                    "The classic low-top sneaker since 1982.", 110m, ShoesCategoryId, NikeBrandId, FootwearTypeId);
                p.SetAttribute(ColorAttrId, "White"); p.SetAttribute(SizeAttrId, "42");
                p.SetAttribute(MaterialAttrId, "Leather"); p.SetAttribute(GenderAttrId, "Unisex");
                p.SetAttribute(ClosureAttrId, "Lace-up"); p.SetAttribute(SoleAttrId, "Rubber");
                yield return p;

                // ── Apparel / Clothing (10) ────────────────────────────────────────
                p = Make("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee09",
                    "NIKE-DRI-BLK-M", "Nike Dri-FIT Training Tee Black", "nike-dri-fit-training-tee-black",
                    "Sweat-wicking fabric keeps you dry during workouts.", 35m, ClothingCategoryId, NikeBrandId, ApparelTypeId);
                p.SetAttribute(ColorAttrId, "Black"); p.SetAttribute(SizeAttrId, "M");
                p.SetAttribute(MaterialAttrId, "100% Polyester"); p.SetAttribute(GenderAttrId, "Men");
                p.SetAttribute(FitAttrId, "Regular"); p.SetAttribute(StyleAttrId, "Sport");
                p.SetAttribute(PatternAttrId, "Solid"); p.SetAttribute(SeasonAttrId, "All Season");
                yield return p;

                p = Make("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee10",
                    "ADIDAS-TREFOIL-WHT-L", "Adidas Trefoil Hoodie White", "adidas-trefoil-hoodie-white",
                    "Iconic Trefoil logo hoodie in premium fleece.", 75m, ClothingCategoryId, AdidasBrandId, ApparelTypeId);
                p.SetAttribute(ColorAttrId, "White"); p.SetAttribute(SizeAttrId, "L");
                p.SetAttribute(MaterialAttrId, "80% Cotton, 20% Polyester"); p.SetAttribute(GenderAttrId, "Unisex");
                p.SetAttribute(FitAttrId, "Regular"); p.SetAttribute(StyleAttrId, "Casual");
                p.SetAttribute(PatternAttrId, "Solid"); p.SetAttribute(SeasonAttrId, "Autumn/Winter");
                yield return p;

                p = Make("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee11",
                    "PUMA-ESS-RED-L", "Puma Essential Logo Tee Red", "puma-essential-logo-tee-red",
                    "Everyday casual tee with embroidered Puma logo.", 30m, ClothingCategoryId, PumaBrandId, ApparelTypeId);
                p.SetAttribute(ColorAttrId, "Red"); p.SetAttribute(SizeAttrId, "L");
                p.SetAttribute(MaterialAttrId, "100% Cotton"); p.SetAttribute(GenderAttrId, "Unisex");
                p.SetAttribute(FitAttrId, "Regular"); p.SetAttribute(StyleAttrId, "Casual");
                p.SetAttribute(PatternAttrId, "Solid"); p.SetAttribute(SeasonAttrId, "Spring/Summer");
                yield return p;

                p = Make("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee12",
                    "NB-ATH-BLU-M", "New Balance Athletics Tee Blue", "new-balance-athletics-tee-blue",
                    "Relaxed fit athletic tee for everyday wear.", 32m, ClothingCategoryId, NewBalanceBrandId, ApparelTypeId);
                p.SetAttribute(ColorAttrId, "Blue"); p.SetAttribute(SizeAttrId, "M");
                p.SetAttribute(MaterialAttrId, "Pima Cotton"); p.SetAttribute(GenderAttrId, "Men");
                p.SetAttribute(FitAttrId, "Relaxed"); p.SetAttribute(StyleAttrId, "Casual");
                p.SetAttribute(PatternAttrId, "Solid"); p.SetAttribute(SeasonAttrId, "Spring/Summer");
                yield return p;

                p = Make("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee13",
                    "CV-STAR-BLK-XL", "Converse Star Chevron Tee Black", "converse-star-chevron-tee-black",
                    "Graphic tee featuring the iconic Converse chevron.", 35m, ClothingCategoryId, ConverseBrandId, ApparelTypeId);
                p.SetAttribute(ColorAttrId, "Black"); p.SetAttribute(SizeAttrId, "XL");
                p.SetAttribute(MaterialAttrId, "100% Cotton"); p.SetAttribute(GenderAttrId, "Unisex");
                p.SetAttribute(FitAttrId, "Regular"); p.SetAttribute(StyleAttrId, "Street");
                p.SetAttribute(PatternAttrId, "Graphic"); p.SetAttribute(SeasonAttrId, "All Season");
                yield return p;

                p = Make("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee14",
                    "NIKE-FLEECE-BLK-L", "Nike Club Fleece Pants Black", "nike-club-fleece-pants-black",
                    "Relaxed jogger pants in soft Club Fleece fabric.", 65m, ClothingCategoryId, NikeBrandId, ApparelTypeId);
                p.SetAttribute(ColorAttrId, "Black"); p.SetAttribute(SizeAttrId, "L");
                p.SetAttribute(MaterialAttrId, "80% Cotton, 20% Polyester"); p.SetAttribute(GenderAttrId, "Men");
                p.SetAttribute(FitAttrId, "Relaxed"); p.SetAttribute(StyleAttrId, "Casual");
                p.SetAttribute(PatternAttrId, "Solid"); p.SetAttribute(SeasonAttrId, "Autumn/Winter");
                yield return p;

                p = Make("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee15",
                    "ADIDAS-SST-BLU-M", "Adidas SST Track Jacket Blue", "adidas-sst-track-jacket-blue",
                    "Retro tricot track jacket with three stripes.", 90m, ClothingCategoryId, AdidasBrandId, ApparelTypeId);
                p.SetAttribute(ColorAttrId, "Blue"); p.SetAttribute(SizeAttrId, "M");
                p.SetAttribute(MaterialAttrId, "100% Polyester Tricot"); p.SetAttribute(GenderAttrId, "Men");
                p.SetAttribute(FitAttrId, "Regular"); p.SetAttribute(StyleAttrId, "Street");
                p.SetAttribute(PatternAttrId, "Striped"); p.SetAttribute(SeasonAttrId, "All Season");
                yield return p;

                p = Make("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee16",
                    "NIKE-GFX-WHT-S", "Nike Sportswear Graphic Tee White", "nike-sportswear-graphic-tee-white",
                    "Soft cotton tee with bold Nike graphic print.", 38m, ClothingCategoryId, NikeBrandId, ApparelTypeId);
                p.SetAttribute(ColorAttrId, "White"); p.SetAttribute(SizeAttrId, "S");
                p.SetAttribute(MaterialAttrId, "100% Cotton"); p.SetAttribute(GenderAttrId, "Unisex");
                p.SetAttribute(FitAttrId, "Regular"); p.SetAttribute(StyleAttrId, "Casual");
                p.SetAttribute(PatternAttrId, "Graphic"); p.SetAttribute(SeasonAttrId, "Spring/Summer");
                yield return p;

                p = Make("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee17",
                    "PUMA-CAT-BLU-M", "Puma Cat Logo Polo Blue", "puma-cat-logo-polo-blue",
                    "Classic piqué polo with embroidered Puma cat.", 45m, ClothingCategoryId, PumaBrandId, ApparelTypeId);
                p.SetAttribute(ColorAttrId, "Blue"); p.SetAttribute(SizeAttrId, "M");
                p.SetAttribute(MaterialAttrId, "100% Cotton Piqué"); p.SetAttribute(GenderAttrId, "Men");
                p.SetAttribute(FitAttrId, "Slim"); p.SetAttribute(StyleAttrId, "Casual");
                p.SetAttribute(PatternAttrId, "Solid"); p.SetAttribute(SeasonAttrId, "Spring/Summer");
                yield return p;

                p = Make("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee18",
                    "NB-HOOD-BLK-L", "New Balance Essential Hoodie Black", "new-balance-essential-hoodie-black",
                    "Pullover hoodie with kangaroo pocket.", 80m, ClothingCategoryId, NewBalanceBrandId, ApparelTypeId);
                p.SetAttribute(ColorAttrId, "Black"); p.SetAttribute(SizeAttrId, "L");
                p.SetAttribute(MaterialAttrId, "French Terry Cotton"); p.SetAttribute(GenderAttrId, "Unisex");
                p.SetAttribute(FitAttrId, "Relaxed"); p.SetAttribute(StyleAttrId, "Casual");
                p.SetAttribute(PatternAttrId, "Solid"); p.SetAttribute(SeasonAttrId, "Autumn/Winter");
                yield return p;

                // ── Bags (5) ───────────────────────────────────────────────────────
                p = Make("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee19",
                    "NIKE-HRTG-BLK", "Nike Heritage Backpack Black", "nike-heritage-backpack-black",
                    "25L backpack with padded laptop sleeve.", 55m, BagsCategoryId, NikeBrandId, AccessoriesTypeId);
                p.SetAttribute(ColorAttrId, "Black"); p.SetAttribute(MaterialAttrId, "600D Polyester");
                p.SetAttribute(GenderAttrId, "Unisex"); p.SetAttribute(CapacityAttrId, "25");
                p.SetAttribute(StrapTypeAttrId, "Backpack"); p.SetAttribute(DimensionsAttrId, "47 × 30 × 15");
                p.SetAttribute(WeightAttrId, "450");
                yield return p;

                p = Make("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee20",
                    "ADIDAS-CLS-BLU", "Adidas Classic Backpack Blue", "adidas-classic-backpack-blue",
                    "Lightweight backpack with front zip pocket.", 50m, BagsCategoryId, AdidasBrandId, AccessoriesTypeId);
                p.SetAttribute(ColorAttrId, "Blue"); p.SetAttribute(MaterialAttrId, "Ripstop Nylon");
                p.SetAttribute(GenderAttrId, "Unisex"); p.SetAttribute(CapacityAttrId, "20");
                p.SetAttribute(StrapTypeAttrId, "Backpack"); p.SetAttribute(DimensionsAttrId, "44 × 28 × 14");
                p.SetAttribute(WeightAttrId, "380");
                yield return p;

                p = Make("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee21",
                    "PUMA-PHASE-RED", "Puma Phase Backpack Red", "puma-phase-backpack-red",
                    "Everyday backpack with ergonomic shoulder straps.", 45m, BagsCategoryId, PumaBrandId, AccessoriesTypeId);
                p.SetAttribute(ColorAttrId, "Red"); p.SetAttribute(MaterialAttrId, "Polyester");
                p.SetAttribute(GenderAttrId, "Unisex"); p.SetAttribute(CapacityAttrId, "22");
                p.SetAttribute(StrapTypeAttrId, "Backpack"); p.SetAttribute(DimensionsAttrId, "43 × 29 × 16");
                p.SetAttribute(WeightAttrId, "400");
                yield return p;

                p = Make("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee22",
                    "NB-ATH-BAG-BLK", "New Balance Athletics Backpack Black", "new-balance-athletics-backpack-black",
                    "Sports backpack with ventilated back panel.", 60m, BagsCategoryId, NewBalanceBrandId, AccessoriesTypeId);
                p.SetAttribute(ColorAttrId, "Black"); p.SetAttribute(MaterialAttrId, "Polyester");
                p.SetAttribute(GenderAttrId, "Unisex"); p.SetAttribute(CapacityAttrId, "24");
                p.SetAttribute(StrapTypeAttrId, "Backpack"); p.SetAttribute(DimensionsAttrId, "46 × 31 × 15");
                p.SetAttribute(WeightAttrId, "420");
                yield return p;

                p = Make("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee23",
                    "CV-SPEED-WHT", "Converse Speed Backpack White", "converse-speed-backpack-white",
                    "Urban backpack with star logo patch.", 48m, BagsCategoryId, ConverseBrandId, AccessoriesTypeId);
                p.SetAttribute(ColorAttrId, "White"); p.SetAttribute(MaterialAttrId, "Canvas");
                p.SetAttribute(GenderAttrId, "Unisex"); p.SetAttribute(CapacityAttrId, "18");
                p.SetAttribute(StrapTypeAttrId, "Backpack"); p.SetAttribute(DimensionsAttrId, "40 × 26 × 13");
                p.SetAttribute(WeightAttrId, "350");
                yield return p;

                // ── Accessories (4) ────────────────────────────────────────────────
                p = Make("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee24",
                    "NIKE-CAP-BLK", "Nike Futura Sports Cap Black", "nike-futura-sports-cap-black",
                    "Structured 6-panel cap with Dri-FIT sweatband.", 28m, AccessoriesCategoryId, NikeBrandId, AccessoriesTypeId);
                p.SetAttribute(ColorAttrId, "Black"); p.SetAttribute(MaterialAttrId, "100% Cotton");
                p.SetAttribute(GenderAttrId, "Unisex"); p.SetAttribute(WeightAttrId, "90");
                yield return p;

                p = Make("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee25",
                    "ADIDAS-CAP-WHT", "Adidas 3-Stripes Cap White", "adidas-3-stripes-cap-white",
                    "Adjustable cotton cap with embroidered 3-Stripes.", 25m, AccessoriesCategoryId, AdidasBrandId, AccessoriesTypeId);
                p.SetAttribute(ColorAttrId, "White"); p.SetAttribute(MaterialAttrId, "100% Cotton");
                p.SetAttribute(GenderAttrId, "Unisex"); p.SetAttribute(WeightAttrId, "85");
                yield return p;

                p = Make("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee26",
                    "PUMA-CAP-BLU", "Puma Running Cap Blue", "puma-running-cap-blue",
                    "Lightweight running cap with UV protection.", 22m, AccessoriesCategoryId, PumaBrandId, AccessoriesTypeId);
                p.SetAttribute(ColorAttrId, "Blue"); p.SetAttribute(MaterialAttrId, "100% Polyester");
                p.SetAttribute(GenderAttrId, "Unisex"); p.SetAttribute(WeightAttrId, "75");
                yield return p;

                p = Make("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee27",
                    "NB-BEAN-BLK", "New Balance Performance Beanie Black", "new-balance-performance-beanie-black",
                    "Thermal knit beanie for cold-weather runs.", 20m, AccessoriesCategoryId, NewBalanceBrandId, AccessoriesTypeId);
                p.SetAttribute(ColorAttrId, "Black"); p.SetAttribute(MaterialAttrId, "Wool / Acrylic Blend");
                p.SetAttribute(GenderAttrId, "Unisex"); p.SetAttribute(WeightAttrId, "80");
                yield return p;

                // ── Sportswear (3) ─────────────────────────────────────────────────
                p = Make("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee28",
                    "NIKE-SHORT-BLK-M", "Nike Pro Training Shorts Black", "nike-pro-training-shorts-black",
                    "7-inch training shorts with built-in liner.", 40m, SportswearCategoryId, NikeBrandId, ApparelTypeId);
                p.SetAttribute(ColorAttrId, "Black"); p.SetAttribute(SizeAttrId, "M");
                p.SetAttribute(MaterialAttrId, "88% Polyester, 12% Elastane"); p.SetAttribute(GenderAttrId, "Men");
                p.SetAttribute(FitAttrId, "Slim"); p.SetAttribute(StyleAttrId, "Sport");
                p.SetAttribute(PatternAttrId, "Solid"); p.SetAttribute(SeasonAttrId, "All Season");
                yield return p;

                p = Make("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee29",
                    "ADIDAS-TECH-BLK-L", "Adidas Techfit Compression Tee Black", "adidas-techfit-compression-tee-black",
                    "Compression fit tee with muscle support zones.", 55m, SportswearCategoryId, AdidasBrandId, ApparelTypeId);
                p.SetAttribute(ColorAttrId, "Black"); p.SetAttribute(SizeAttrId, "L");
                p.SetAttribute(MaterialAttrId, "76% Polyester, 24% Elastane"); p.SetAttribute(GenderAttrId, "Men");
                p.SetAttribute(FitAttrId, "Slim"); p.SetAttribute(StyleAttrId, "Sport");
                p.SetAttribute(PatternAttrId, "Solid"); p.SetAttribute(SeasonAttrId, "All Season");
                yield return p;

                p = Make("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee30",
                    "PUMA-EVIDE-BLU-M", "Puma Evide Track Pants Blue", "puma-evide-track-pants-blue",
                    "Slim-fit track pants with side stripe detail.", 60m, SportswearCategoryId, PumaBrandId, ApparelTypeId);
                p.SetAttribute(ColorAttrId, "Blue"); p.SetAttribute(SizeAttrId, "M");
                p.SetAttribute(MaterialAttrId, "100% Polyester"); p.SetAttribute(GenderAttrId, "Men");
                p.SetAttribute(FitAttrId, "Slim"); p.SetAttribute(StyleAttrId, "Sport");
                p.SetAttribute(PatternAttrId, "Striped"); p.SetAttribute(SeasonAttrId, "All Season");
                yield return p;
            }
        }

        private static Product Make(string id, string sku, string name, string slug,
            string? desc, decimal price, Guid categoryId, Guid brandId, Guid productTypeId)
            => Product.Create(new Guid(id), sku, name, slug, desc, price,
                categoryId, brandId, productTypeId, ProductStatus.Active);
    }
}
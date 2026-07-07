namespace Catalog.Data.Seed
{
    public static class InitialData
    {
        // ProductTypes
        public static readonly Guid FashionTypeId = new("11111111-1111-1111-1111-111111111111");

        // Categories
        public static readonly Guid ClothingCategoryId    = new("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1");
        public static readonly Guid ShoesCategoryId       = new("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2");
        public static readonly Guid AccessoriesCategoryId = new("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3");
        public static readonly Guid BagsCategoryId        = new("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4");
        public static readonly Guid SportswearCategoryId  = new("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5");

        // Brands
        public static readonly Guid NikeBrandId        = new("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1");
        public static readonly Guid AdidasBrandId      = new("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2");
        public static readonly Guid PumaBrandId        = new("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3");
        public static readonly Guid NewBalanceBrandId  = new("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb4");
        public static readonly Guid ConverseBrandId    = new("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb5");

        // AttributeDefinitions
        public static readonly Guid ColorAttrId    = new("cccccccc-cccc-cccc-cccc-000000000001");
        public static readonly Guid SizeAttrId     = new("cccccccc-cccc-cccc-cccc-000000000002");
        public static readonly Guid MaterialAttrId = new("cccccccc-cccc-cccc-cccc-000000000003");

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
            AttributeDefinition.Create(ColorAttrId, "color", "Color", AttributeDataType.Enum,
                allowedValues: ["Black", "White", "Red", "Blue", "Green", "Navy", "Grey", "Beige", "Brown", "Pink"]),
            AttributeDefinition.Create(SizeAttrId, "size", "Size", AttributeDataType.Enum,
                allowedValues: ["XS", "S", "M", "L", "XL", "XXL", "36", "37", "38", "39", "40", "41", "42", "43", "44", "One Size"]),
            AttributeDefinition.Create(MaterialAttrId, "material", "Material", AttributeDataType.Text),
        ];

        public static IEnumerable<ProductType> ProductTypes
        {
            get
            {
                var fashion = ProductType.Create(FashionTypeId, "fashion", "Fashion");
                fashion.AttachAttribute(ColorAttrId, isRequired: true, displayOrder: 1);
                fashion.AttachAttribute(SizeAttrId, isRequired: true, displayOrder: 2);
                fashion.AttachAttribute(MaterialAttrId, isRequired: false, displayOrder: 3);
                yield return fashion;
            }
        }

        public static IEnumerable<Product> Products
        {
            get
            {
                // ── Shoes (8) ──────────────────────────────────────────────────────────
                yield return Shoe("5334c996-8457-4cf0-815c-ed2b77c4ff61",
                    "NIKE-AM90-BLK-42", "Nike Air Max 90 Black", "nike-air-max-90-black",
                    "Classic Nike Air Max 90 sneakers.", 129m, NikeBrandId,
                    "Black", "42", "Mesh / Leather");

                yield return Shoe("c67d6323-e8b1-4bdf-9a75-b0d0d2e7e914",
                    "ADIDAS-SS-WHT-40", "Adidas Stan Smith White", "adidas-stan-smith-white",
                    "Iconic Adidas Stan Smith sneakers.", 99m, AdidasBrandId,
                    "White", "40", "Leather");

                yield return Shoe("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee03",
                    "NIKE-REACT-BLU-41", "Nike React Infinity Run Blue", "nike-react-infinity-run-blue",
                    "Lightweight running shoe designed for endurance.", 159m, NikeBrandId,
                    "Blue", "41", "Flyknit");

                yield return Shoe("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee04",
                    "ADIDAS-UB22-BLK-42", "Adidas Ultraboost 22 Black", "adidas-ultraboost-22-black",
                    "Premium running shoe with Boost cushioning.", 189m, AdidasBrandId,
                    "Black", "42", "Primeknit");

                yield return Shoe("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee05",
                    "PUMA-RSX-WHT-41", "Puma RS-X White", "puma-rs-x-white",
                    "Retro-inspired chunky sneaker.", 109m, PumaBrandId,
                    "White", "41", "Mesh / Synthetic");

                yield return Shoe("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee06",
                    "NB-990V5-BLK-40", "New Balance 990v5 Black", "new-balance-990v5-black",
                    "Made in USA. Premium suede upper.", 175m, NewBalanceBrandId,
                    "Black", "40", "Suede / Mesh");

                yield return Shoe("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee07",
                    "CV-CHUCK-BLK-39", "Converse Chuck Taylor All Star Black", "converse-chuck-taylor-all-star-black",
                    "The original canvas high-top sneaker.", 65m, ConverseBrandId,
                    "Black", "39", "Canvas");

                yield return Shoe("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee08",
                    "NIKE-AF1-WHT-42", "Nike Air Force 1 White", "nike-air-force-1-white",
                    "The classic low-top sneaker since 1982.", 110m, NikeBrandId,
                    "White", "42", "Leather");

                // ── Clothing (10) ──────────────────────────────────────────────────────
                yield return Apparel("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee09",
                    "NIKE-DRI-BLK-M", "Nike Dri-FIT Training Tee Black", "nike-dri-fit-training-tee-black",
                    "Sweat-wicking fabric keeps you dry during workouts.", 35m, NikeBrandId,
                    "Black", "M", "100% Polyester");

                yield return Apparel("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee10",
                    "ADIDAS-TREFOIL-WHT-L", "Adidas Trefoil Hoodie White", "adidas-trefoil-hoodie-white",
                    "Iconic Trefoil logo hoodie in premium fleece.", 75m, AdidasBrandId,
                    "White", "L", "80% Cotton, 20% Polyester");

                yield return Apparel("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee11",
                    "PUMA-ESS-RED-L", "Puma Essential Logo Tee Red", "puma-essential-logo-tee-red",
                    "Everyday casual tee with embroidered Puma logo.", 30m, PumaBrandId,
                    "Red", "L", "100% Cotton");

                yield return Apparel("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee12",
                    "NB-ATH-BLU-M", "New Balance Athletics Tee Blue", "new-balance-athletics-tee-blue",
                    "Relaxed fit athletic tee for everyday wear.", 32m, NewBalanceBrandId,
                    "Blue", "M", "Pima Cotton");

                yield return Apparel("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee13",
                    "CV-STAR-BLK-XL", "Converse Star Chevron Tee Black", "converse-star-chevron-tee-black",
                    "Graphic tee featuring the iconic Converse chevron.", 35m, ConverseBrandId,
                    "Black", "XL", "100% Cotton");

                yield return Apparel("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee14",
                    "NIKE-FLEECE-BLK-L", "Nike Club Fleece Pants Black", "nike-club-fleece-pants-black",
                    "Relaxed jogger pants in soft Club Fleece fabric.", 65m, NikeBrandId,
                    "Black", "L", "80% Cotton, 20% Polyester");

                yield return Apparel("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee15",
                    "ADIDAS-SST-BLU-M", "Adidas SST Track Jacket Blue", "adidas-sst-track-jacket-blue",
                    "Retro tricot track jacket with three stripes.", 90m, AdidasBrandId,
                    "Blue", "M", "100% Polyester Tricot");

                yield return Apparel("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee16",
                    "NIKE-GFX-WHT-S", "Nike Sportswear Graphic Tee White", "nike-sportswear-graphic-tee-white",
                    "Soft cotton tee with bold Nike graphic print.", 38m, NikeBrandId,
                    "White", "S", "100% Cotton");

                yield return Apparel("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee17",
                    "PUMA-CAT-BLU-M", "Puma Cat Logo Polo Blue", "puma-cat-logo-polo-blue",
                    "Classic piqué polo with embroidered Puma cat.", 45m, PumaBrandId,
                    "Blue", "M", "100% Cotton Piqué");

                yield return Apparel("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee18",
                    "NB-HOOD-BLK-L", "New Balance Essential Hoodie Black", "new-balance-essential-hoodie-black",
                    "Pullover hoodie with kangaroo pocket.", 80m, NewBalanceBrandId,
                    "Black", "L", "French Terry Cotton");

                // ── Bags (5) ───────────────────────────────────────────────────────────
                yield return Bag("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee19",
                    "NIKE-HRTG-BLK", "Nike Heritage Backpack Black", "nike-heritage-backpack-black",
                    "25L backpack with padded laptop sleeve.", 55m, NikeBrandId, "Black");

                yield return Bag("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee20",
                    "ADIDAS-CLS-BLU", "Adidas Classic Backpack Blue", "adidas-classic-backpack-blue",
                    "Lightweight backpack with front zip pocket.", 50m, AdidasBrandId, "Blue");

                yield return Bag("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee21",
                    "PUMA-PHASE-RED", "Puma Phase Backpack Red", "puma-phase-backpack-red",
                    "Everyday backpack with ergonomic shoulder straps.", 45m, PumaBrandId, "Red");

                yield return Bag("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee22",
                    "NB-ATH-BAG-BLK", "New Balance Athletics Backpack Black", "new-balance-athletics-backpack-black",
                    "Sports backpack with ventilated back panel.", 60m, NewBalanceBrandId, "Black");

                yield return Bag("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee23",
                    "CV-SPEED-WHT", "Converse Speed Backpack White", "converse-speed-backpack-white",
                    "Urban backpack with star logo patch.", 48m, ConverseBrandId, "White");

                // ── Accessories (4) ────────────────────────────────────────────────────
                yield return Accessory("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee24",
                    "NIKE-CAP-BLK", "Nike Futura Sports Cap Black", "nike-futura-sports-cap-black",
                    "Structured 6-panel cap with Dri-FIT sweatband.", 28m, NikeBrandId, "Black");

                yield return Accessory("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee25",
                    "ADIDAS-CAP-WHT", "Adidas 3-Stripes Cap White", "adidas-3-stripes-cap-white",
                    "Adjustable cotton cap with embroidered 3-Stripes.", 25m, AdidasBrandId, "White");

                yield return Accessory("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee26",
                    "PUMA-CAP-BLU", "Puma Running Cap Blue", "puma-running-cap-blue",
                    "Lightweight running cap with UV protection.", 22m, PumaBrandId, "Blue");

                yield return Accessory("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee27",
                    "NB-BEAN-BLK", "New Balance Performance Beanie Black", "new-balance-performance-beanie-black",
                    "Thermal knit beanie for cold-weather runs.", 20m, NewBalanceBrandId, "Black");

                // ── Sportswear (3) ─────────────────────────────────────────────────────
                yield return Apparel("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee28",
                    "NIKE-SHORT-BLK-M", "Nike Pro Training Shorts Black", "nike-pro-training-shorts-black",
                    "7-inch training shorts with built-in liner.", 40m, NikeBrandId,
                    "Black", "M", "88% Polyester, 12% Elastane", SportswearCategoryId);

                yield return Apparel("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee29",
                    "ADIDAS-TECH-BLK-L", "Adidas Techfit Compression Tee Black", "adidas-techfit-compression-tee-black",
                    "Compression fit tee with muscle support zones.", 55m, AdidasBrandId,
                    "Black", "L", "76% Polyester, 24% Elastane", SportswearCategoryId);

                yield return Apparel("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee30",
                    "PUMA-EVIDE-BLU-M", "Puma Evide Track Pants Blue", "puma-evide-track-pants-blue",
                    "Slim-fit track pants with side stripe detail.", 60m, PumaBrandId,
                    "Blue", "M", "100% Polyester", SportswearCategoryId);
            }
        }

        private static Product Shoe(string id, string sku, string name, string slug,
            string? desc, decimal price, Guid brandId,
            string color, string size, string? material = null)
        {
            var p = Make(id, sku, name, slug, desc, price, ShoesCategoryId, brandId);
            p.SetAttribute(ColorAttrId, color);
            p.SetAttribute(SizeAttrId, size);
            if (material is not null) p.SetAttribute(MaterialAttrId, material);
            return p;
        }

        private static Product Apparel(string id, string sku, string name, string slug,
            string? desc, decimal price, Guid brandId,
            string color, string size, string? material = null,
            Guid? categoryId = null)
        {
            var p = Make(id, sku, name, slug, desc, price, categoryId ?? ClothingCategoryId, brandId);
            p.SetAttribute(ColorAttrId, color);
            p.SetAttribute(SizeAttrId, size);
            if (material is not null) p.SetAttribute(MaterialAttrId, material);
            return p;
        }

        private static Product Bag(string id, string sku, string name, string slug,
            string? desc, decimal price, Guid brandId, string color)
        {
            var p = Make(id, sku, name, slug, desc, price, BagsCategoryId, brandId);
            p.SetAttribute(ColorAttrId, color);
            p.SetAttribute(SizeAttrId, "One Size");
            return p;
        }

        private static Product Accessory(string id, string sku, string name, string slug,
            string? desc, decimal price, Guid brandId, string color)
        {
            var p = Make(id, sku, name, slug, desc, price, AccessoriesCategoryId, brandId);
            p.SetAttribute(ColorAttrId, color);
            p.SetAttribute(SizeAttrId, "One Size");
            return p;
        }

        private static Product Make(string id, string sku, string name, string slug,
            string? desc, decimal price, Guid categoryId, Guid brandId)
            => Product.Create(new Guid(id), sku, name, slug, desc, price,
                categoryId, brandId, FashionTypeId, ProductStatus.Active);
    }
}
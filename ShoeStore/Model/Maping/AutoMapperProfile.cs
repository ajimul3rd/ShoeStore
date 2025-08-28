using AutoMapper;
using ShoeStore.Model.Entity;
using ShoeStore.Shared.Dto;

namespace ShoeStore.Maping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Category Mappings
            CreateMap<Categories, CategoriesDto>();
            CreateMap<CategoriesDto, Categories>()
                .ForMember(dest => dest.Product, opt => opt.Ignore());

            // Product Mappings
            CreateMap<Product, ProductDto>();
            CreateMap<ProductDto, Product>()
                .ForMember(dest => dest.Categories, opt => opt.Ignore())
                .ForMember(dest => dest.ProductDetails, opt => opt.Ignore())
                .ForMember(dest => dest.ProductVariant, opt => opt.Ignore());

            // Master Data Mappings
            CreateMap<MasterCategories, MasterCategoriesDto>();
            CreateMap<MasterCategoriesDto, MasterCategories>();
            
            CreateMap<MasterColorPalette, MasterColorPaletteDto>();
            CreateMap<MasterColorPaletteDto, MasterColorPalette>();
            
            CreateMap<MasterSizeChart, MasterSizeChartDto>();
            CreateMap<MasterSizeChartDto, MasterSizeChart>();

            // Order Mappings
            CreateMap<Order, OrderDto>();
            CreateMap<OrderDto, Order>()
                .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
                .ForMember(dest => dest.ShippingAddress, opt => opt.Ignore())
                .ForMember(dest => dest.StatusHistory, opt => opt.Ignore())
                .ForMember(dest => dest.Users, opt => opt.Ignore());

            CreateMap<OrderItem, OrderItemDto>();
            CreateMap<OrderItemDto, OrderItem>()
                .ForMember(dest => dest.Order, opt => opt.Ignore())
                .ForMember(dest => dest.ProductVariant, opt => opt.Ignore())
                .ForMember(dest => dest.SizeVariant, opt => opt.Ignore());

            CreateMap<OrderStatusHistory, OrderStatusHistoryDto>();
            CreateMap<OrderStatusHistoryDto, OrderStatusHistory>()
                .ForMember(dest => dest.Order, opt => opt.Ignore());

            // Product Detail Mappings
            CreateMap<ProductDetails, ProductDetailsDto>();
            CreateMap<ProductDetailsDto, ProductDetails>()
                .ForMember(dest => dest.Product, opt => opt.Ignore());

            CreateMap<ProductVariant, ProductVariantDto>();
            CreateMap<ProductVariantDto, ProductVariant>()
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.ImageUrls, opt => opt.Ignore())
                .ForMember(dest => dest.SizeVariant, opt => opt.Ignore());

            // Address Mappings
            CreateMap<ShippingAddress, ShippingAddressDto>();
            CreateMap<ShippingAddressDto, ShippingAddress>()
                .ForMember(dest => dest.Users, opt => opt.Ignore());

            // Shopping Cart Mappings
            CreateMap<ShoppingCart, ShoppingCartDto>();
            CreateMap<ShoppingCartDto, ShoppingCart>()
                .ForMember(dest => dest.Users, opt => opt.Ignore())
                .ForMember(dest => dest.CartItems, opt => opt.Ignore());

            CreateMap<SizeVariant, SizeVariantDto>();
            CreateMap<SizeVariantDto, SizeVariant>()
                .ForMember(dest => dest.ProductVariant, opt => opt.Ignore());

            // User Mappings
            CreateMap<Users, UsersDto>();
            CreateMap<UsersDto, Users>()
                .ForMember(dest => dest.Address, opt => opt.Ignore());

            // Wishlist Mappings
            CreateMap<Wishlist, WishlistDto>();
            CreateMap<WishlistDto, Wishlist>()
                .ForMember(dest => dest.Users, opt => opt.Ignore())
                .ForMember(dest => dest.ProductVariant, opt => opt.Ignore());
        }
    }
}

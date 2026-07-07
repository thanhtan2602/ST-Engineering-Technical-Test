import { NavLink } from 'react-router-dom';
import { Package, Tag, Layers, ListTree, Puzzle } from 'lucide-react';
import { cn } from '@/shared/utils/cn';

const navItems = [
  { to: '/products', label: 'Products', icon: Package },
  { to: '/categories', label: 'Categories', icon: ListTree },
  { to: '/brands', label: 'Brands', icon: Tag },
  { to: '/product-types', label: 'Product Types', icon: Layers },
  { to: '/attribute-definitions', label: 'Attributes', icon: Puzzle },
];

export function Sidebar() {
  return (
    <aside className="hidden w-60 shrink-0 border-r bg-card md:flex md:flex-col">
      <div className="flex h-16 items-center gap-2 border-b px-6">
        <div className="grid h-8 w-8 place-items-center rounded-md bg-primary text-primary-foreground">
          <Package className="h-4 w-4" />
        </div>
        <span className="text-sm font-semibold">Fashion Shop</span>
      </div>
      <nav className="flex-1 space-y-1 px-3 py-4">
        {navItems.map((item) => (
          <NavLink
            key={item.to}
            to={item.to}
            className={({ isActive }) =>
              cn(
                'flex items-center gap-3 rounded-md px-3 py-2 text-sm font-medium transition-colors',
                isActive
                  ? 'bg-primary/10 text-primary'
                  : 'text-muted-foreground hover:bg-accent hover:text-accent-foreground',
              )
            }
          >
            <item.icon className="h-4 w-4" />
            {item.label}
          </NavLink>
        ))}
      </nav>
      <div className="border-t p-4 text-xs text-muted-foreground">
        v0.1.0 · ST Engineering
      </div>
    </aside>
  );
}

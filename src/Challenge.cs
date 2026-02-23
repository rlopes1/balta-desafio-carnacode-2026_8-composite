// DESAFIO: Sistema de Menus Hierárquicos
// PROBLEMA: Um sistema de gestão de conteúdo precisa construir menus com itens simples e submenus aninhados
// O código atual trata itens individuais e grupos de forma diferente, complicando operações recursivas

using System;
using System.Collections.Generic;

namespace DesignPatternChallenge
{
    // Contexto: Sistema CMS que precisa renderizar menus complexos com múltiplos níveis
    // Alguns itens são links simples, outros são menus que contêm mais itens


    // 1. Identifique o padrão de projeto que pode resolver esse problema
    // 2. Crie uma interface ou classe base comum que MenuItem e MenuGroup possam implementar
    // 3. Refatore as classes para usar essa interface/classe base
    // 4. Atualize o MenuManager para trabalhar com a interface/classe base
    // 5. Mantenha a funcionalidade original (renderização, contagem, operações em lote)

    public abstract class MenuComponent
    {
        public string Title{get;set;}
        public bool IsActive {get;set;}
        public string Icon {get;set;}

        protected MenuComponent (string title, string icon = "")
        {
            Title = title;
            IsActive = true;
            Icon = icon;
        }
        public abstract void Render(int indent = 0);
        public abstract int CountItems();
        public abstract void DisableAllItems();
        public abstract MenuComponent FindItemByUrl(string url);

    }




    public class MenuItem : MenuComponent
    {
        public string Url {get;set;}

        public MenuItem(string title, string url, string icon = "") : base(title, icon)
        {
            Url = url;
        }

        public override void Render(int indent = 0)
        {
            var indentation = new string(' ', indent * 2);
            var activeStatus = IsActive ? "✓" : "✗";
            Console.WriteLine($"{indentation}[{activeStatus}] {Icon} {Title} → {Url}");
        }

        public override int CountItems()
        {
            return 1;
        }

        public override void DisableAllItems()
        {
            IsActive = false;
        }

        public override MenuComponent FindItemByUrl(string url)
        {
            if (Url == url)
            {
                return this;
            }
            return null;
        }
    }

    public class MenuGroup : MenuComponent
    {
        public List<MenuComponent> children { get; set; } = new();

        public MenuGroup(string title, string icon = "") : base(title, icon)
        {
            
        }

        // Problema: Lógica complexa para renderizar itens e subgrupos
        public override void Render(int indent = 0)
        {
            var indentation = new string(' ', indent * 2);
            var activeStatus = IsActive ? "✓" : "✗";
            Console.WriteLine($"{indentation}[{activeStatus}] {Icon} {Title} ▼");

            // Precisa iterar sobre duas coleções diferentes
            foreach (var item in children)
            {
                item.Render(indent + 1);
            }
            
        }

        // Problema: Contagem recursiva complexa
        public override int CountItems()
        {
            return children.Sum(x => x.CountItems());
        }

        // Problema: Operações em lote exigem código duplicado
        public override void DisableAllItems()
        {
            foreach (var item in children)
            {
                item.DisableAllItems();
            }
            IsActive = false;
        }

        public override MenuComponent FindItemByUrl(string url)
        {
           foreach (var item in children)
            {
                var foundItem = item.FindItemByUrl(url);
                if (foundItem != null)
                {
                    return foundItem;
                }
            }
            return null;

            // children.Select(x => x.FindItemByUrl(url)).FirstOrDefault(x => x != null)
        }
    }

    public class MenuManager
    {
        private List<MenuComponent> _topLevelItems;

        public MenuManager()
        {
            _topLevelItems = new List<MenuComponent>();
        }

        // Problema: Precisa gerenciar dois tipos diferentes no nível raiz
        public void AddItem(MenuComponent item)
        {
            _topLevelItems.Add(item);
        }

        // Problema: Renderização trata itens e grupos separadamente
        public void RenderMenu()
        {
            Console.WriteLine("=== Menu Principal ===\n");

            foreach (var item in _topLevelItems)
            {
                item.Render();
            }
        }

        // Problema: Operações precisam iterar sobre ambas as coleções
        public int GetTotalItems()
        {
            return _topLevelItems.Sum(x => x.CountItems());
        }

        // Problema: Busca em toda hierarquia é complicada
        public MenuComponent FindItemByUrl(string url)
        {
            return _topLevelItems.Select(x => x.FindItemByUrl(url)).FirstOrDefault(x => x != null);
        }

       
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Sistema de Menus CMS ===\n");

            var manager = new MenuManager();

            // Item simples no nível raiz
            manager.AddItem(new MenuItem("Home", "/", "🏠"));

            // Grupo com itens
            var productsMenu = new MenuGroup("Produtos", "📦");
            productsMenu.children.Add(new MenuItem("Todos", "/produtos"));
            productsMenu.children.Add(new MenuItem("Categorias", "/categorias"));
            productsMenu.children.Add(new MenuItem("Ofertas", "/ofertas"));

            // Subgrupo dentro de grupo
            var clothingMenu = new MenuGroup("Roupas", "👕");
            clothingMenu.children.Add(new MenuItem("Camisetas", "/roupas/camisetas"));
            clothingMenu.children.Add(new MenuItem("Calças", "/roupas/calcas"));
            productsMenu.children.Add(clothingMenu);

            manager.AddItem(productsMenu);

            // Outro grupo
            var adminMenu = new MenuGroup("Administração", "⚙️");
            adminMenu.children.Add(new MenuItem("Usuários", "/admin/usuarios"));
            adminMenu.children.Add(new MenuItem("Configurações", "/admin/config"));
            manager.AddItem(adminMenu);

            manager.RenderMenu();

            Console.WriteLine($"\nTotal de itens no menu: {manager.GetTotalItems()}");

            // Problema: Buscar item requer lógica especial para navegar hierarquia
            var item = manager.FindItemByUrl("/roupas/camisetas");
            if (item != null)
            {
                Console.WriteLine($"\n✓ Item encontrado: {item.Title}");
            }

            Console.WriteLine("\n=== PROBLEMAS ===");
            Console.WriteLine("✗ MenuItem e MenuGroup são tratados de forma diferente");
            Console.WriteLine("✗ Operações recursivas requerem código duplicado");
            Console.WriteLine("✗ Cliente precisa saber se está lidando com item ou grupo");
            Console.WriteLine("✗ Adicionar nova operação = modificar ambas as classes");
            Console.WriteLine("✗ Não há interface uniforme para tratar a hierarquia");

            // Perguntas para reflexão:
            // - Como tratar itens individuais e grupos de forma uniforme?
            // - Como simplificar operações recursivas na hierarquia?
            // - Como permitir que o cliente trate toda a estrutura sem saber os detalhes?
            // - Como facilitar adicionar novas operações que percorrem a árvore?
        }
    }
}

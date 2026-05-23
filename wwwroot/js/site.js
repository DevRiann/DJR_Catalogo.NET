document.addEventListener("DOMContentLoaded", function () {
    // 📍 Pega o caminho da página atual (ex: "/Sobre")
    const paginaAtual = window.location.pathname;

    // 🔍 Seleciona os links do menu usando as classes do Bootstrap
    const links = document.querySelectorAll('.navbar-nav .nav-link');
    
    let linkAtivo = null;

    // 🚥 Aplica a lógica condicional que você pensou:
    if (paginaAtual.includes("Sobre")) {
        linkAtivo = Array.from(links).find(link => link.getAttribute("asp-page") === "/Sobre" || link.href.includes("Sobre"));
    } else if (paginaAtual.includes("Contato")) {
        linkAtivo = Array.from(links).find(link => link.getAttribute("asp-page") === "/Contato" || link.href.includes("Contato"));
    } else {
        // Se não for nenhum dos dois, assume que é a Home
        linkAtivo = Array.from(links).find(link => link.getAttribute("asp-page") === "/Index" || link.pathname === "/" || link.href.endsWith("/"));
    }

    // ✨ Se encontrar o link correspondente, adiciona a classe de destaque
    if (linkAtivo) {
        linkAtivo.classList.add("menu-ativo");
    }
});

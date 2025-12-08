(function () {
    const seasonSelect = document.getElementById('season-select');
    const prevBtn = document.getElementById('prev-page');
    const nextBtn = document.getElementById('next-page');
    const pageInfoEl = document.getElementById('page-info');
    const episodesListEl = document.getElementById('episodes-list');
    const charactersListEl = document.getElementById('characters-list');
    const modalEl = document.getElementById('character-modal');
    const modalBackdropEl = modalEl ? modalEl.querySelector('.rm-modal-backdrop') : null;
    const modalCloseEl = document.getElementById('modal-close');
    const modalBodyEl = document.getElementById('modal-body');
    const locationMapEl = document.getElementById('location-map');
    const genderFilter = document.getElementById('gender-filter');
    const statusFilter = document.getElementById('status-filter');

    let currentCharactersRaw = [];


    let currentPage = 1;
    let totalPages = 1;
    let currentSeason = '';
    let selectedEpisodeId = null;

    function mapStatusToPt(status) {
        const normalized = (status || '').toLowerCase();
        switch (normalized) {
            case 'alive':
                return 'Vivo';
            case 'dead':
                return 'Morto';
            case 'unknown':
            default:
                return 'Desconhecido';
        }
    }

    function isDead(status) {
        return (status || '').toLowerCase() === 'dead';
    }
    function normalizeLocationName(name) {
        const trimmed = (name || '').trim();
        if (!trimmed || trimmed.toLowerCase() === 'unknown') {
            return 'Desconhecida';
        }
        return trimmed;
    }


    async function loadEpisodes() {
        const params = new URLSearchParams();
        params.append('page', currentPage);
        if (currentSeason) {
            params.append('season', currentSeason);
        }

        setLoading(episodesListEl, 'Carregando episódios...');

        try {
            const response = await fetch('/api/episodes?' + params.toString());
            if (!response.ok) {
                throw new Error('Falha ao carregar episódios');
            }
            const data = await response.json();
            totalPages = data.totalPages || 1;
            renderEpisodes(data.items || []);
            updatePagination();
        } catch (error) {
            console.error(error);
            episodesListEl.innerHTML = '<p class="rm-error">Erro ao carregar episódios.</p>';
        }
    }

    function renderEpisodes(episodes) {
        if (!episodes || episodes.length === 0) {
            episodesListEl.innerHTML = '<p class="rm-placeholder">Nenhum episódio encontrado.</p>';
            return;
        }

        episodesListEl.innerHTML = '';
        episodes.forEach(ep => {
            const card = document.createElement('button');
            card.type = 'button';
            card.className = 'rm-episode-card';
            if (ep.id === selectedEpisodeId) {
                card.classList.add('rm-episode-card--selected');
            }
            card.dataset.episodeId = ep.id;

            card.innerHTML = `
                <div class="rm-episode-title">${ep.episodeCode} - ${ep.name}</div>
                <div class="rm-episode-meta">
                    <span>${ep.airDate}</span>
                    <span>${ep.characterCount} pers.</span>
                </div>
            `;

            card.addEventListener('click', () => {
                selectedEpisodeId = ep.id;
                highlightSelectedEpisode();
                loadCharactersForEpisode(ep.id);
            });

            episodesListEl.appendChild(card);
        });
    }

    function highlightSelectedEpisode() {
        const cards = episodesListEl.querySelectorAll('.rm-episode-card');
        cards.forEach(card => {
            const id = Number(card.dataset.episodeId);
            if (id === selectedEpisodeId) {
                card.classList.add('rm-episode-card--selected');
            } else {
                card.classList.remove('rm-episode-card--selected');
            }
        });
    }

    function updatePagination() {
        if (!pageInfoEl || !prevBtn || !nextBtn) {
            return;
        }

        pageInfoEl.textContent = `Página ${currentPage} de ${totalPages}`;
        prevBtn.disabled = currentPage <= 1;
        nextBtn.disabled = currentPage >= totalPages;
    }

    async function loadCharactersForEpisode(episodeId) {
        setLoading(charactersListEl, 'Carregando personagens...');
        try {
            const response = await fetch(`/api/episodes/${episodeId}/characters`);
            if (!response.ok) {
                throw new Error('Falha ao carregar personagens');
            }
            const data = await response.json();
            currentCharactersRaw = data || [];
            applyFiltersAndRender();
            renderCharacters(data);
            renderLocationMap(data);
        } catch (error) {
            console.error(error);
            charactersListEl.innerHTML = '<p class="rm-error">Erro ao carregar personagens.</p>';
        }
    }

    function applyFiltersAndRender() {
        if (!currentCharactersRaw || currentCharactersRaw.length === 0) {
            renderCharacters([]);
            renderLocationMap([]);
            return;
        }

        let filtered = currentCharactersRaw.slice();

        const genderVal = (genderFilter?.value || '').toLowerCase();
        const statusVal = (statusFilter?.value || '').toLowerCase();

        if (genderVal) {
            filtered = filtered.filter(ch =>
                (ch.gender || '').toLowerCase() === genderVal
            );
        }

        if (statusVal) {
            filtered = filtered.filter(ch =>
                (ch.status || '').toLowerCase() === statusVal
            );
        }

        renderCharacters(filtered);
        renderLocationMap(filtered);
    }


    function renderCharacters(characters) {
        if (!characters || characters.length === 0) {
            charactersListEl.innerHTML = '<p class="rm-placeholder">Nenhum personagem para este episódio.</p>';
            return;
        }

        charactersListEl.innerHTML = '';
        characters.forEach(ch => {
            const card = document.createElement('button');
            card.type = 'button';
            card.className = 'rm-character-card';
            card.dataset.characterId = ch.id;

            if (isDead(ch.status)) {
                card.classList.add('rm-character-card--dead');
            }

            const statusLabel = mapStatusToPt(ch.status);

            card.innerHTML = `
                <div class="rm-character-avatar-wrapper">
                    <img class="rm-character-avatar" src="${ch.imageUrl}" alt="${ch.name}" />
                </div>
                <div class="rm-character-name">${ch.name}</div>
                <div class="rm-character-meta">${statusLabel} • ${ch.species}</div>
            `;

            card.addEventListener('click', () => {
                openCharacterModal(ch.id);
            });

            charactersListEl.appendChild(card);
        });
    }
    
    function renderLocationMap(characters) {
        if (!locationMapEl) {
            return;
        }

        if (!characters || characters.length === 0) {
            locationMapEl.innerHTML = '';
            return;
        }

        // mapa de nome -> { name, originCount, currentCount }
        const locationMap = {};

        characters.forEach(ch => {
            const originName = normalizeLocationName(ch.originName);
            const currentName = normalizeLocationName(ch.locationName);

            if (!locationMap[originName]) {
                locationMap[originName] = { name: originName, originCount: 0, currentCount: 0 };
            }
            locationMap[originName].originCount++;

            if (!locationMap[currentName]) {
                locationMap[currentName] = { name: currentName, originCount: 0, currentCount: 0 };
            }
            locationMap[currentName].currentCount++;
        });

        const locations = Object.values(locationMap);

        // ordenar, deixando "Desconhecida" por último
        locations.sort((a, b) => {
            if (a.name === 'Desconhecida') return 1;
            if (b.name === 'Desconhecida') return -1;
            return a.name.localeCompare(b.name);
        });

        if (locations.length === 0) {
            locationMapEl.innerHTML = '';
            return;
        }

        let columnsHtml = '';
        locations.forEach(loc => {
            const hasOrigin = loc.originCount > 0;
            const hasCurrent = loc.currentCount > 0;

            columnsHtml += `
  <div class="rm-location-column">
      ${hasOrigin ? `
      <div class="rm-location-box rm-location-box--origin">
          <div class="rm-location-box-label">Origem</div>
          <div class="rm-location-box-name" title="${loc.name}">${loc.name}</div>
          <div class="rm-location-box-count">${loc.originCount} pers.</div>
      </div>
      ` : '<div class="rm-location-box-spacer"></div>'}

      ${hasOrigin ? '<div class="rm-location-connector"></div>'
                    : '<div class="rm-location-connector-spacer"></div>'}

      <div class="rm-location-node" title="${loc.name}"></div>

      ${hasCurrent ? '<div class="rm-location-connector"></div>'
                    : '<div class="rm-location-connector-spacer"></div>'}

      ${hasCurrent ? `
      <div class="rm-location-box rm-location-box--current">
          <div class="rm-location-box-label">Local</div>
          <div class="rm-location-box-name" title="${loc.name}">${loc.name}</div>
          <div class="rm-location-box-count">${loc.currentCount} pers.</div>
      </div>
      ` : '<div class="rm-location-box-spacer"></div>'}
  </div>
`;

        });



        locationMapEl.innerHTML = `
        <h3 class="rm-location-title">Mapa de localizações do episódio</h3>
        <div class="rm-location-columns">
            ${columnsHtml}
        </div>
    `;
    }


    async function openCharacterModal(characterId) {
        try {
            modalBodyEl.innerHTML = '<p class="rm-loading">Carregando personagem...</p>';
            showModal();

            const response = await fetch(`/api/characters/${characterId}`);
            if (!response.ok) {
                throw new Error('Falha ao carregar personagem');
            }
            const ch = await response.json();
            renderCharacterDetail(ch);
        } catch (error) {
            console.error(error);
            modalBodyEl.innerHTML = '<p class="rm-error">Erro ao carregar personagem.</p>';
        }
    }

    function renderCharacterDetail(ch) {
        const typePart = ch.type ? ` • ${ch.type}` : '';
        const statusLabel = mapStatusToPt(ch.status);
        modalBodyEl.innerHTML = `
            <div class="rm-modal-header">
                <img class="rm-modal-avatar" src="${ch.imageUrl}" alt="${ch.name}" />
                <div>
                    <h2 class="rm-modal-title">${ch.name}</h2>
                    <p class="rm-modal-subtitle">${statusLabel} • ${ch.species}${typePart}</p>
                </div>
            </div>
            <div class="rm-modal-row">
                <span class="rm-label">Gênero</span>
                <span>${ch.gender || 'Desconhecido'}</span>
            </div>
            <div class="rm-modal-row">
                <span class="rm-label">Origem</span>
                <span>${ch.originName || 'Desconhecida'}</span>
            </div>
            <div class="rm-modal-row">
                <span class="rm-label">Última localização conhecida</span>
                <span>${ch.locationName || 'Desconhecida'}</span>
            </div>
        `;
    }

    function setLoading(container, text) {
        container.innerHTML = `<p class="rm-loading">${text}</p>`;
    }

    function showModal() {
        modalEl.classList.remove('rm-hidden');
    }

    function hideModal() {
        modalEl.classList.add('rm-hidden');
    }

    function init() {
        if (!episodesListEl) {
            return;
        }

        if (seasonSelect) {
            seasonSelect.addEventListener('change', () => {
                currentSeason = seasonSelect.value;
                currentPage = 1;
                loadEpisodes();
            });
        }

        if (prevBtn) {
            prevBtn.addEventListener('click', () => {
                if (currentPage > 1) {
                    currentPage--;
                    loadEpisodes();
                }
            });
        }

        if (nextBtn) {
            nextBtn.addEventListener('click', () => {
                if (currentPage < totalPages) {
                    currentPage++;
                    loadEpisodes();
                }
            });
        }

        if (modalCloseEl) {
            modalCloseEl.addEventListener('click', hideModal);
        }
        if (modalBackdropEl) {
            modalBackdropEl.addEventListener('click', hideModal);
        }
        if (genderFilter) {
            genderFilter.addEventListener('change', applyFiltersAndRender);
        }
        if (statusFilter) {
            statusFilter.addEventListener('change', applyFiltersAndRender);
        }
        document.addEventListener('keydown', (event) => {
            if (event.key === 'Escape' && !modalEl.classList.contains('rm-hidden')) {
                hideModal();
            }
        });

        loadEpisodes();
    }

    document.addEventListener('DOMContentLoaded', init);
})();

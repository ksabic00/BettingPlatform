// src/app/app.config.ts
import { APP_INITIALIZER, ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideClientHydration } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptorsFromDi, HTTP_INTERCEPTORS } from '@angular/common/http';

import { routes } from './app.routes';

import { BASE_PATH, Configuration, PlayersService } from './api';
import { ApiInterceptor } from './core/http/api.interceptor';
import { environment } from '../enviroments/enviroment';
import { firstValueFrom } from 'rxjs';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideClientHydration(),
    provideRouter(routes),

    provideHttpClient(withInterceptorsFromDi()),
    { provide: HTTP_INTERCEPTORS, useClass: ApiInterceptor, multi: true },

    { provide: BASE_PATH, useValue: environment.apiBaseUrl },
    { provide: Configuration, useValue: new Configuration({ basePath: environment.apiBaseUrl }) },
    { provide: APP_INITIALIZER, useFactory: ensurePlayer, deps: [PlayersService], multi: true }
  ]
};

function ensurePlayer(players: PlayersService) {
  return async () => {
    const existing = localStorage.getItem('playerId');
    if (existing) return;
    const body = { displayName: 'Guest-' + Math.floor(Math.random() * 1_000_000) };
    const id = await firstValueFrom(players.apiPlayersPost(body));
    localStorage.setItem('playerId', String(id));
  };
}
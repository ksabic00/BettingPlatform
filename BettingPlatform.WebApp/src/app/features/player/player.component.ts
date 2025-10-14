import { Component, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PlayersService, CreatePlayerCommand } from '../../api';

@Component({
  selector: 'app-player',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './player.component.html'
})
export class PlayerComponent {
  displayName = '';
  messages: string[] = [];
  playerId?: string;

  constructor(private players: PlayersService, @Inject(PLATFORM_ID) private pid: Object) {
    if (isPlatformBrowser(this.pid)) this.playerId = localStorage.getItem('playerId') ?? undefined;
  }

  create() {
    const cmd: CreatePlayerCommand = { displayName: this.displayName };
    this.players.apiPlayersPost(cmd).subscribe({
      next: (id: string) => {
        if (isPlatformBrowser(this.pid)) localStorage.setItem('playerId', id);
        this.playerId = id;
        this.messages = ['Player created.'];
      },
      error: (e: any) => this.messages = e.messages ?? ['Error']
    });
  }
}

import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PlayersService, CreatePlayerCommand, PlayerSummaryDto } from '../../api';
import { PlayerContextService, CurrentPlayer } from '../../core/state/player-context.service';
import { ToastService } from '../../core/ui/toast.service';

@Component({
  standalone: true,
  selector: 'app-players',
  imports: [CommonModule, FormsModule],
  templateUrl: './player.component.html',
})
export class PlayerComponent implements OnInit {
  players: PlayerSummaryDto[] = [];
  current: CurrentPlayer = null;

  selectedId: string | null = null; 
  name = '';                        
  msg = '';

  constructor(private api: PlayersService, private ctx: PlayerContextService, private toast: ToastService) {}

  ngOnInit(): void {
    this.ctx.current$.subscribe(p => {
      this.current = p;
      if (!this.selectedId && p) this.selectedId = p.id;
    });

    this.refresh();
  }

  refresh() {
    this.api.apiPlayersGet().subscribe(list => {
      this.players = list ?? [];
      if (!this.current && this.players.length === 1) {
        this.selectedId = this.players[0].id!;
      }
    });
  }

  applySelection() {
    const p = this.players.find(x => x.id === this.selectedId!);
    if (!p) return;
    this.ctx.set({ id: p.id!, displayName: p.displayName! });
    this.toast.info(`Playing as: ${p.displayName}`);
  }

  create() {
    const displayName = this.name.trim();
    if (!displayName) { this.toast.warning('Enter display name.'); return; }
  
    this.api.apiPlayersPost({ displayName }).subscribe({
      next: id => {
        this.name = '';
        this.toast.success('Player created.');
        this.refresh();
        this.ctx.set({ id: id!, displayName });
        this.selectedId = id!;
      },
      error: e => (e?.messages ?? ['Create failed.']).forEach((m: string) => this.toast.error(m))
    });
  }
}

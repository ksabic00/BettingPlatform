import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { isPlatformBrowser } from '@angular/common';
import { OfferDto, OfferOutcomeDto } from '../../api';

export interface SlipSelection {
  offerId: string;
  outcomeTemplateId: string;
  marketCode: string;
  outcomeCode: string;
  odds: number;
  match: string;            
  startsAtUtc?: string;
}

type Persisted = { stake: number; selections: SlipSelection[] };

const KEY = 'betSlip';

@Injectable({ providedIn: 'root' })
export class BetslipService {
  private _selections = new BehaviorSubject<SlipSelection[]>([]);
  private _stake = new BehaviorSubject<number>(10);

  selections$ = this._selections.asObservable();
  stake$ = this._stake.asObservable();

  constructor(@Inject(PLATFORM_ID) private pid: Object) {
    if (isPlatformBrowser(pid)) {
      const raw = localStorage.getItem(KEY);
      if (raw) {
        try {
          const data = JSON.parse(raw) as Persisted;
          this._stake.next(data.stake ?? 10);
          this._selections.next(data.selections ?? []);
        } catch { /* ignore */ }
      }
      this.persist();
      this._selections.subscribe(() => this.persist());
      this._stake.subscribe(() => this.persist());
    }
  }

  private persist() {
    if (!isPlatformBrowser(this.pid)) return;
    const data: Persisted = { stake: this._stake.value, selections: this._selections.value };
    localStorage.setItem(KEY, JSON.stringify(data));
  }

  clear() { this._selections.next([]); }

  setStake(v: number) { this._stake.next(Math.max(0, v || 0)); }

  addOrReplace(offer: OfferDto, outcome: OfferOutcomeDto) {
    const s: SlipSelection = {
      offerId: offer.offerId!,
      outcomeTemplateId: outcome.outcomeTemplateId!,
      marketCode: offer.marketCode!,
      outcomeCode: outcome.outcomeCode!,
      odds: Number(outcome.odds),
      match: `${offer.homeTeam} vs ${offer.awayTeam}`,
      startsAtUtc: offer.startsAtUtc as any
    };
    const rest = this._selections.value.filter(x => x.offerId !== s.offerId);
    this._selections.next([...rest, s]);
  }

  remove(offerId: string) {
    this._selections.next(this._selections.value.filter(x => x.offerId !== offerId));
  }

  combinedOdds(): number {
    const prod = this._selections.value.reduce((acc, s) => acc * Number(s.odds), 1);
    return Math.round(prod * 100) / 100;
  }


getSnapshot() {
    return { stake: this._stake.value, selections: this._selections.value };
  }
  
}
